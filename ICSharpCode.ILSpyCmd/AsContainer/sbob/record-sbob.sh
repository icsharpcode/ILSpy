#!/usr/bin/env bash
# Records the runtime behavior of the ilspycmd container image and writes it as a Software Bill
# of Behavior (cp-ilspycmd.yaml next to this script). See ../README.md, section "SBOB".
#
# Runs on a Linux host (here: inside `podman machine ssh`) with: podman, sudo, Inspektor Gadget
# `ig` (https://inspektor-gadget.io, root, kernel >= 5.10 with BTF) and `bobctl`
# (https://github.com/k8sstormcenter/bob/releases). Needs a Debug build of ILSpy.sln for the input
# assemblies.
#
# The image is started once with a sleeping entrypoint, ig attaches to it (rootless containers
# started after ig are not detected), and every ilspycmd invocation of the benign suite runs
# through `podman exec`, each once with and once without the nuget.org update check.
set -euo pipefail

HERE=$(cd "$(dirname "$0")" && pwd)
ROOT=$(cd "$HERE/../../.." && pwd)
WORK=${SBOB_WORK:-$HOME/ilspycmd-sbob}
IMAGE=${SBOB_IMAGE:-ilspycmd:111forautomation}
SOCK=${PODMAN_SOCK:-/run/user/$(id -u)/podman/podman.sock}
IG=${IG:-ig}
BOBCTL=${BOBCTL:-bobctl}
C=ilspycmd-sbob
OUT=${SBOB_OUT:-$HERE/cp-ilspycmd.yaml}
for t in podman sudo "$IG" "$BOBCTL"; do command -v "$t" >/dev/null || { echo "missing: $t" >&2; exit 1; }; done

BIN=$ROOT/ICSharpCode.ILSpyCmd/bin/Debug/net10.0
for f in "$BIN/ilspycmd.dll" "$BIN/ilspycmd.pdb" "$BIN/ICSharpCode.Decompiler.dll" "$BIN/ICSharpCode.ILSpyX.dll" "$ROOT/ILSpy/bin/Debug/net10.0/ILSpy.dll"; do
  [ -f "$f" ] || { echo "missing input $f: run build.ps1 -Configuration Debug first" >&2; exit 1; }
done

echo "== image"
podman build -q -t "$IMAGE" -f "$HERE/../DockerfileForAutomation" "$HERE/.."
IMAGE_ID=$(podman image inspect "$IMAGE" --format '{{.Id}}')
BASE=$(podman image inspect "$IMAGE" --format '{{index .Annotations "org.opencontainers.image.base.digest"}}')
TOOL_VERSION=$(sed -n 's/.*--version \([0-9.]*\).*/\1/p' "$HERE/../DockerfileForAutomation")

echo "== inputs -> $WORK"
# Files the container user wrote belong to a subordinate uid of the rootless Podman user;
# only that user namespace may delete them.
[ -d "$WORK" ] && podman unshare rm -rf "$WORK"
mkdir -p "$WORK/infolder/refs" "$WORK/outfolder" "$WORK/nugetcache" "$WORK/raw" "$WORK/cp"
cp "$BIN/ilspycmd.dll" "$BIN/ilspycmd.pdb" "$BIN/ICSharpCode.Decompiler.dll" "$ROOT/ILSpy/bin/Debug/net10.0/ILSpy.dll" "$WORK/infolder/"
cp "$BIN/ICSharpCode.Decompiler.dll" "$BIN/ICSharpCode.ILSpyX.dll" "$WORK/infolder/refs/"
chmod -R a+rwX "$WORK/outfolder" "$WORK/nugetcache"

echo "== container"
podman rm -f "$C" >/dev/null 2>&1 || true
podman run -d --name "$C" --entrypoint /bin/sleep -v "$WORK/infolder:/infolder:ro" -v "$WORK/outfolder:/outfolder" \
  -v "$WORK/nugetcache:/home/ilspy/.local/share/NuGet/http-cache" "$IMAGE" infinity >/dev/null
# ilspycmd 11.1 runs its update check through NuGet.Protocol, whose http-cache the image build
# already populated. Mounting an empty directory over it records the fetch path; aging the
# entries later (from the host, so no extra process runs in the traced container) records the
# rewrite path NuGet takes for entries older than 30 minutes.

echo "== gadgets"
gadget() { # name, output file, extra flags...
  local g=$1 out=$2; shift 2
  sudo "$IG" run "$g:latest" --runtimes podman --podman-socketpath "$SOCK" -c "$C" "$@" > "$WORK/raw/$out" 2> "$WORK/raw/$g.err" &
}
gadget trace_exec trace_exec.json -o json --paths
gadget trace_open trace_open.json -o json --paths
gadget trace_tcp trace_tcp.json -o json
gadget trace_dns trace_dns.json -o json
gadget trace_capabilities trace_capabilities.json -o json --unique
gadget advise_seccomp advise_seccomp.txt
sleep 20   # gadget images load; nothing is traced before that

echo "== benign suite"
TOOL=/home/ilspy/.dotnet/tools/ilspycmd
x() { # the DockerfileForAutomation entrypoint is `bash -l -c`; mirror it
  local rc=0
  podman exec "$C" /bin/bash -l -c "$TOOL $*" > /dev/null 2>&1 || rc=$?
  printf '%3d  ilspycmd %s\n' "$rc" "$*" | tee -a "$WORK/suite.log"
}
IN=/infolder/ilspycmd.dll
for u in "" "--disable-updatecheck"; do
  x $u --version
  x $u --help
  x $u $IN
  x $u -o /outfolder/single $IN
  x $u -p -o /outfolder/proj $IN
  x $u --nested-directories -p -o /outfolder/nested $IN
  x $u -t ICSharpCode.ILSpyCmd.ILSpyCmdProgram $IN
  x $u -m "'M:ICSharpCode.ILSpyCmd.DotNetToolUpdateChecker.LatestStableVersion(System.Collections.Generic.IEnumerable{System.String})'" $IN
  x $u -il $IN
  x $u --il-sequence-points $IN
  x $u -l c -l i -l s -l d -l e $IN
  x $u --list-resources /infolder/ILSpy.dll
  x $u --resource ICSharpCode.ILSpy.Properties.Resources.resources/OpenFrom -o /outfolder/res /infolder/ILSpy.dll
  x $u --dump-table TypeDef --json $IN
  x $u -genpdb -o /outfolder/pdb $IN
  x $u -usepdb $IN
  x $u -r /infolder/refs -lv CSharp7_3 -ds AlwaysUseBraces=true $IN
  x $u --no-dead-code --no-dead-stores --ignore-decompilation-errors $IN
  x $u --generate-diagrammer -o /outfolder/diag /infolder/ICSharpCode.Decompiler.dll
  x $u /infolder/ICSharpCode.Decompiler.dll -o /outfolder/decompiler
  x $u /infolder/does-not-exist.dll
  x $u --no-such-option $IN
done
podman unshare find "$WORK/nugetcache" -type f -exec touch -d "40 minutes ago" {} +
x -l c $IN

echo "== stop"
# advise_seccomp prints the profile of a still-running container only when it is interrupted,
# so it is stopped first and on its own, and the recording fails rather than ships no syscalls.
sleep 2
sudo pkill -INT -f "$IG run advise_seccomp" || true
for i in $(seq 30); do [ -s "$WORK/raw/advise_seccomp.txt" ] && break; sleep 1; done
[ -s "$WORK/raw/advise_seccomp.txt" ] || { echo "advise_seccomp produced no profile" >&2; exit 1; }
sudo pkill -INT -f "$IG run" || true
wait
podman rm -f "$C" >/dev/null

echo "== convert"
SDK=$(sed -n 's/^FROM //p' "$HERE/../DockerfileForAutomation")
podman run --rm -v "$HERE:/src:ro" -v "$WORK:/work" "$SDK" dotnet run /src/IgToContainerProfile.cs -- \
  --in /work/raw --out /work/cp/cp-ilspycmd.yaml --name ilspycmd --namespace ilspycmd \
  --image-id "localhost/$IMAGE@$IMAGE_ID" --image-tag "$IMAGE base=$BASE ilspycmd=$TOOL_VERSION" \
  --anchor /infolder --anchor /outfolder --anchor /tmp

echo "== validate"
cp "$WORK/cp/cp-ilspycmd.yaml" "$OUT"
"$BOBCTL" validate --file "$OUT"

echo "== no-network check (not part of the SBOB; the update check must fail silently)"
rc=0; podman run --rm --network none -v "$WORK/infolder:/infolder:ro" "$IMAGE" "$TOOL -t ICSharpCode.ILSpyCmd.ProgramExitCodes $IN" > /dev/null || rc=$?
echo "exit $rc with --network none" | tee -a "$WORK/suite.log"
echo "done: $OUT (raw recordings in $WORK)"
