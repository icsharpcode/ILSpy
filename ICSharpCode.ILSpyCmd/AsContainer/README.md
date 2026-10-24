# ILSpyCmd in Docker

Inspired by https://trustedsec.com/blog/hunting-deserialization-vulnerabilities-with-claude (and thus https://github.com/berdav/ilspycmd-docker)

## Building the Image

There are two dockerfiles available - one for interactive use of ilspycmd (exploration), and the other for driving it from the outside (automation)

### Interactive 

`docker build -t ilspycmd:111interactive . -f Dockerfile`

`docker run --rm -it -v ./:/docker ilspycmd:111interactive`

### Automation

`docker build -t ilspycmd:111forautomation . -f DockerfileForAutomation`

`docker run --rm -v ./:/infolder -v ./out:/outfolder ilspycmd:111forautomation "/home/ilspy/.dotnet/tools/ilspycmd -p -o /outfolder /infolder/sample.dll"`

## Software Bill of Behavior (SBOB)

`sbob/cp-ilspycmd.yaml` declares what the automation image does at runtime: the binaries it
executes, the files it opens, the Linux capabilities and syscalls it uses, and its only network
peer (`api.nuget.org`, for the update check that `--disable-updatecheck` turns off). It is a
[Kubescape](https://kubescape.io/docs/operator/bill-of-behavior/) `ContainerProfile` in the
format of the [SBOB specification](https://billofbehavior.com/bob/docs/spec/) (v0.0.4, proposed),
the runtime companion of an SBOM: a runtime verifier such as Kubescape's node-agent flags any
behavior the document does not declare.

It was recorded, not written: `sbob/record-sbob.sh` starts the image, attaches
[Inspektor Gadget](https://inspektor-gadget.io) (the eBPF tracer Kubescape itself learns
from) to the container, runs the common `ilspycmd` invocations with and without the update
check, converts the events with `sbob/IgToContainerProfile.cs` (a .NET file-based app), and
checks the result with `bobctl validate` from
[k8sstormcenter/bob](https://github.com/k8sstormcenter/bob).
Re-run it after changing the Dockerfile or bumping the tool version; it needs a Linux host
with podman, sudo, `ig` and `bobctl` (on Windows: `podman machine ssh`, the repo is at
`/mnt/<drive>/...`) and a Debug build for the input assemblies.

The document binds to the image this directory builds, not to a published one: `imageID` is
the local image id and `imageTag` records the base-image digest and tool version. To use it
in a cluster, apply it before the workload and label the pod
`kubescape.io/user-defined-profile: ilspycmd` (rename `metadata.namespace` to the pod's
namespace first). Signing (`sign-object`, spec section 8) is not done here.

Not covered by the recording: `-d`/`--bundle-entry` (no single-file bundle fixture), and the
Native AOT `ilspycmd.linux-x64` package, which is not published yet and will run without the
`dotnet` host, so its profile will be smaller.
