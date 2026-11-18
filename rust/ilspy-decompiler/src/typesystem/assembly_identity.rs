//! Assembly identities as the managed metadata reads them: the full name of an assembly
//! definition (`MetadataReader.GetFullAssemblyName`) and of an AssemblyRef row
//! (`AssemblyReference.GetFullAssemblyName`), and `TypeSystemExtensions.IsDirectImportOf`.

use crate::metadata::Metadata;
use crate::metadata::tables::{TableId, rows};

use super::compilation::{Compilation, ModuleId, TypeDefId};
use super::names::FullTypeName;

/// `AssemblyFlags.PublicKey`.
const PUBLIC_KEY: u32 = 0x1;
/// `AssemblyFlags.Retargetable`.
const RETARGETABLE: u32 = 0x100;

/// SHA-1 (`Sha1ForNonSecretPurposes`), of which a strong-name public key token is made.
fn sha1(data: &[u8]) -> [u8; 20] {
    let mut h: [u32; 5] = [
        0x6745_2301,
        0xEFCD_AB89,
        0x98BA_DCFE,
        0x1032_5476,
        0xC3D2_E1F0,
    ];
    let mut message = data.to_vec();
    message.push(0x80);
    while message.len() % 64 != 56 {
        message.push(0);
    }
    message.extend_from_slice(&((data.len() as u64) * 8).to_be_bytes());
    for block in message.as_chunks::<64>().0 {
        let mut w = [0u32; 80];
        for (i, word) in block.as_chunks::<4>().0.iter().enumerate() {
            w[i] = u32::from_be_bytes(*word);
        }
        for i in 16..80 {
            w[i] = (w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16]).rotate_left(1);
        }
        let [mut a, mut b, mut c, mut d, mut e] = h;
        for (i, &wi) in w.iter().enumerate() {
            let (f, k) = match i {
                0..=19 => ((b & c) | (!b & d), 0x5A82_7999),
                20..=39 => (b ^ c ^ d, 0x6ED9_EBA1),
                40..=59 => ((b & c) | (b & d) | (c & d), 0x8F1B_BCDC),
                _ => (b ^ c ^ d, 0xCA62_C1D6),
            };
            let t = a
                .rotate_left(5)
                .wrapping_add(f)
                .wrapping_add(e)
                .wrapping_add(k)
                .wrapping_add(wi);
            e = d;
            d = c;
            c = b.rotate_left(30);
            b = a;
            a = t;
        }
        for (x, y) in h.iter_mut().zip([a, b, c, d, e]) {
            *x = x.wrapping_add(y);
        }
    }
    let mut out = [0u8; 20];
    for (chunk, x) in out.as_chunks_mut::<4>().0.iter_mut().zip(h) {
        *chunk = x.to_be_bytes();
    }
    out
}

fn hex(bytes: &[u8]) -> String {
    bytes.iter().map(|b| format!("{b:02x}")).collect()
}

/// `CalculatePublicKeyToken`: the last 8 bytes of the key's SHA-1, reversed.
fn public_key_token(key: &[u8]) -> String {
    let hash = sha1(key);
    let mut token = hash[12..].to_vec();
    token.reverse();
    hex(&token)
}

fn culture(md: &Metadata<'_>, idx: crate::metadata::heaps::StrIdx) -> Option<String> {
    Some(if idx.0 == 0 {
        "neutral".to_owned()
    } else {
        md.string(idx).ok()?.into_owned()
    })
}

/// `IModule.FullAssemblyName` of an assembly; `None` for a netmodule or unreadable metadata.
pub fn full_assembly_name(md: &Metadata<'_>) -> Option<String> {
    if md.tables.rows(TableId::Assembly) == 0 {
        return None;
    }
    let a = md.get::<rows::Assembly>(1).ok()?;
    // `AssemblyFlags.PublicKey` does not apply to assembly definitions.
    let key = if a.public_key.0 == 0 {
        "null".to_owned()
    } else {
        public_key_token(md.blob(a.public_key).ok()?)
    };
    Some(format!(
        "{}, Version={}.{}.{}.{}, Culture={}, PublicKeyToken={key}",
        md.string(a.name).ok()?,
        a.major,
        a.minor,
        a.build,
        a.revision,
        culture(md, a.culture)?,
    ))
}

/// `IAssemblyReference.FullName` of an AssemblyRef row.
pub fn assembly_ref_full_name(md: &Metadata<'_>, row: u32) -> Option<String> {
    let r = md.get::<rows::AssemblyRef>(row).ok()?;
    let mut s = format!(
        "{}, Version={}.{}.{}.{}, Culture={}, PublicKeyToken=",
        md.string(r.name).ok()?,
        r.major,
        r.minor,
        r.build,
        r.revision,
        culture(md, r.culture)?,
    );
    if r.public_key_or_token.0 == 0 {
        s.push_str("null");
    } else if r.flags & PUBLIC_KEY != 0 {
        s.push_str(&public_key_token(md.blob(r.public_key_or_token).ok()?));
    } else {
        s.push_str(&hex(md.blob(r.public_key_or_token).ok()?));
    }
    if r.flags & RETARGETABLE != 0 {
        s.push_str(", Retargetable=true");
    }
    Some(s)
}

/// `IsDirectImportOf(type, module)`: whether `module` references the assembly that defines
/// `type`, directly or through a `netstandard` facade that forwards it.
pub fn is_direct_import_of(
    comp: &Compilation<'_>,
    ty: TypeDefId,
    name: &FullTypeName,
    module: ModuleId,
) -> bool {
    let defining = full_assembly_name(&comp.module(ty.module).md);
    let md = &comp.module(module).md;
    for row in 1..=md.tables.rows(TableId::AssemblyRef) {
        let Some(reference) = assembly_ref_full_name(md, row) else {
            continue;
        };
        if defining.as_ref() == Some(&reference) {
            return true;
        }
        let netstandard = md.get::<rows::AssemblyRef>(row).is_ok_and(|r| {
            r.public_key_or_token.0 != 0 && md.string(r.name).is_ok_and(|n| n == "netstandard")
        });
        if netstandard {
            // `FindModuleByReference`: the first module of that full name, else of that simple
            // name, ignoring case.
            let referenced = comp
                .module_ids()
                .find(|&m| {
                    full_assembly_name(&comp.module(m).md)
                        .is_some_and(|n| n.to_uppercase() == reference.to_uppercase())
                })
                .or_else(|| {
                    comp.module_ids()
                        .find(|&m| comp.module(m).assembly_name.to_uppercase() == "NETSTANDARD")
                });
            if referenced.is_some_and(|m| comp.has_type_forwarder(m, name)) {
                return true;
            }
        }
    }
    false
}

#[cfg(test)]
mod tests {
    use super::{hex, public_key_token, sha1};

    #[test]
    fn sha1_matches_the_standard_vectors() {
        assert_eq!(hex(&sha1(b"")), "da39a3ee5e6b4b0d3255bfef95601890afd80709");
        assert_eq!(
            hex(&sha1(b"abc")),
            "a9993e364706816aba3e25717850c26c9cd0d89d"
        );
        assert_eq!(
            hex(&sha1(
                b"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"
            )),
            "84983e441c3bd26ebaae4aa1f95129e5e54670f1"
        );
    }

    #[test]
    fn the_ecma_key_has_the_well_known_token() {
        // The ECMA standard public key, whose token is b77a5c561934e089.
        let ecma = [0u8, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0];
        assert_eq!(public_key_token(&ecma), "b77a5c561934e089");
    }
}
