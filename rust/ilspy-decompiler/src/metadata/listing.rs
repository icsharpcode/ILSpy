//! A canonical text listing of everything the metadata reader decodes: table and heap sizes,
//! every row of every table with its columns, decoded signatures, the lookups that tie tables
//! together, and method bodies.
//!
//! `ICSharpCode.Decompiler.Rust.Tests/MetadataListing.cs` produces the same format from
//! System.Reflection.Metadata; the `MetadataListingEquality` test diffs the two. Any change
//! here must be mirrored there.
//!
//! Conventions: tokens are 8 lowercase hex digits and keep their table when the row is 0;
//! blobs are lowercase hex; a column that fails to decode prints `!bad`; lists are `[a,b]`.

use std::fmt::Write;

use super::bytes::Bytes;
use super::heaps::format_guid;
use super::signature::{self, HandleKind, MethodSig, SigType};
use super::tables::{TableId, rows, token};
use super::{Error, Metadata, Result};

const MAX_NAME_DEPTH: u32 = 64;

/// The listing of an image, or the line for an image that is not readable metadata.
pub fn listing(image: &[u8]) -> String {
    match Metadata::parse(image) {
        Ok(md) => Lister { md: &md }.write(),
        Err(Error::NotManaged) => "NOT MANAGED\n".to_owned(),
        Err(_) => "BAD IMAGE\n".to_owned(),
    }
}

struct Lister<'m, 'a> {
    md: &'m Metadata<'a>,
}

fn or_bad<T: std::fmt::Display>(r: Result<T>) -> String {
    match r {
        Ok(v) => v.to_string(),
        Err(_) => "!bad".to_owned(),
    }
}

fn hex(bytes: &[u8]) -> String {
    bytes
        .iter()
        .fold(String::with_capacity(bytes.len() * 2), |mut s, b| {
            let _ = write!(s, "{b:02x}");
            s
        })
}

fn tok(t: u32) -> String {
    format!("{t:08x}")
}

fn tokens(table: TableId, rows: impl IntoIterator<Item = u32>) -> String {
    let list: Vec<String> = rows.into_iter().map(|r| tok(token(table, r))).collect();
    format!("[{}]", list.join(","))
}

fn fnv1a(bytes: &[u8]) -> u64 {
    bytes.iter().fold(0xcbf2_9ce4_8422_2325, |h, &b| {
        (h ^ u64::from(b)).wrapping_mul(0x0000_0100_0000_01b3)
    })
}

impl<'a> Lister<'_, 'a> {
    fn write(&self) -> String {
        let md = self.md;
        let t = &md.tables;
        let mut out = String::new();
        let mut line = |s: String| {
            out.push_str(&s);
            out.push('\n');
        };
        line(format!("METADATA {}", md.version));
        line(format!(
            "CLI flags={:08x} entry={:08x}",
            md.pe.cli.flags, md.pe.cli.entry_point
        ));
        line(format!(
            "HEAP strings={} blob={} guid={} us={}",
            md.strings.len(),
            md.blobs.len(),
            md.guids.len(),
            md.user_strings.len()
        ));
        for &table in TableId::ALL {
            line(format!(
                "TABLE {:02x} rows={} size={}",
                table as u8,
                t.rows(table),
                t.row_size(table)
            ));
        }
        for row in 1..=t.rows(TableId::Module) {
            line(self.module(row));
        }
        for row in 1..=t.rows(TableId::TypeRef) {
            line(self.type_ref(row));
        }
        for row in 1..=t.rows(TableId::TypeDef) {
            line(self.type_def(row));
        }
        for row in 1..=t.rows(TableId::Field) {
            line(self.field(row));
        }
        for row in 1..=t.rows(TableId::MethodDef) {
            line(self.method(row));
            for body_line in self.body(row) {
                line(body_line);
            }
        }
        for row in 1..=t.rows(TableId::Param) {
            line(self.param(row));
        }
        for row in 1..=t.rows(TableId::InterfaceImpl) {
            let r = md.get::<rows::InterfaceImpl>(row);
            line(format!(
                "IFACE {} iface={}",
                tok(token(TableId::InterfaceImpl, row)),
                or_bad(r.and_then(|r| r.interface.token()).map(tok))
            ));
        }
        for row in 1..=t.rows(TableId::MemberRef) {
            line(self.member_ref(row));
        }
        for row in 1..=t.rows(TableId::Constant) {
            let r = md.get::<rows::Constant>(row);
            line(format!(
                "CONSTANT {} type={} parent={} value={}",
                tok(token(TableId::Constant, row)),
                or_bad(r.clone().map(|r| format!("{:02x}", r.kind & 0xFF))),
                or_bad(r.clone().and_then(|r| r.parent.token()).map(tok)),
                or_bad(r.and_then(|r| md.blob(r.value)).map(hex))
            ));
        }
        for row in 1..=t.rows(TableId::CustomAttribute) {
            let r = md.get::<rows::CustomAttribute>(row);
            line(format!(
                "CA {} parent={} ctor={} value={}",
                tok(token(TableId::CustomAttribute, row)),
                or_bad(r.clone().and_then(|r| r.parent.token()).map(tok)),
                or_bad(r.clone().and_then(|r| r.constructor.token()).map(tok)),
                or_bad(r.and_then(|r| md.blob(r.value)).map(hex))
            ));
        }
        for row in 1..=t.rows(TableId::DeclSecurity) {
            let r = md.get::<rows::DeclSecurity>(row);
            line(format!(
                "DECLSEC {} action={} parent={} perms={}",
                tok(token(TableId::DeclSecurity, row)),
                or_bad(r.clone().map(|r| r.action)),
                or_bad(r.clone().and_then(|r| r.parent.token()).map(tok)),
                or_bad(r.and_then(|r| md.blob(r.permission_set)).map(hex))
            ));
        }
        for row in 1..=t.rows(TableId::StandAloneSig) {
            let sig = md
                .get::<rows::StandAloneSig>(row)
                .and_then(|r| md.blob(r.signature))
                .and_then(|blob| {
                    let header = *blob.first().ok_or(Error::Truncated)?;
                    let mut b = Bytes::new(blob);
                    match signature::header_kind(header) {
                        signature::HEADER_KIND_METHOD => Ok(format!(
                            "method {}",
                            self.method_sig(&signature::decode_method(&mut b)?)?
                        )),
                        signature::HEADER_KIND_LOCALS => Ok(format!(
                            "locals({})",
                            self.types(&signature::decode_locals(&mut b)?)?
                        )),
                        _ => Err(Error::Malformed("stand-alone signature kind")),
                    }
                });
            line(format!(
                "STANDALONESIG {} sig={}",
                tok(token(TableId::StandAloneSig, row)),
                or_bad(sig)
            ));
        }
        for row in 1..=t.rows(TableId::Event) {
            line(self.event(row));
        }
        for row in 1..=t.rows(TableId::Property) {
            line(self.property(row));
        }
        for row in 1..=t.rows(TableId::MethodImpl) {
            let r = md.get::<rows::MethodImpl>(row);
            line(format!(
                "METHODIMPL {} class={} body={} decl={}",
                tok(token(TableId::MethodImpl, row)),
                or_bad(r.clone().map(|r| tok(token(TableId::TypeDef, r.class)))),
                or_bad(r.clone().and_then(|r| r.body.token()).map(tok)),
                or_bad(r.and_then(|r| r.declaration.token()).map(tok))
            ));
        }
        for row in 1..=t.rows(TableId::ModuleRef) {
            let name = md
                .get::<rows::ModuleRef>(row)
                .and_then(|r| md.string(r.name));
            line(format!(
                "MODULEREF {} name={}",
                tok(token(TableId::ModuleRef, row)),
                or_bad(name)
            ));
        }
        for row in 1..=t.rows(TableId::TypeSpec) {
            let sig = md
                .get::<rows::TypeSpec>(row)
                .and_then(|r| md.blob(r.signature))
                .and_then(|blob| self.ty(&signature::decode_type(&mut Bytes::new(blob))?, 0));
            line(format!(
                "TYPESPEC {} sig={}",
                tok(token(TableId::TypeSpec, row)),
                or_bad(sig)
            ));
        }
        for row in 1..=t.rows(TableId::Assembly) {
            line(self.assembly(row));
        }
        for row in 1..=t.rows(TableId::AssemblyRef) {
            line(self.assembly_ref(row));
        }
        for row in 1..=t.rows(TableId::File) {
            let r = md.get::<rows::File>(row);
            line(format!(
                "FILE {} meta={} name={} hash={}",
                tok(token(TableId::File, row)),
                or_bad(r.clone().map(|r| u8::from(r.flags & 1 == 0))),
                or_bad(r.clone().and_then(|r| md.string(r.name))),
                or_bad(r.and_then(|r| md.blob(r.hash_value)).map(hex))
            ));
        }
        for row in 1..=t.rows(TableId::ExportedType) {
            let r = md.get::<rows::ExportedType>(row);
            line(format!(
                "EXPORTEDTYPE {} flags={} typedef={} name={} ns={} impl={}",
                tok(token(TableId::ExportedType, row)),
                or_bad(r.clone().map(|r| format!("{:08x}", r.flags))),
                or_bad(r.clone().map(|r| format!("{:08x}", r.type_def_id))),
                or_bad(r.clone().and_then(|r| md.string(r.name))),
                or_bad(r.clone().and_then(|r| md.string(r.namespace))),
                or_bad(r.and_then(|r| r.implementation.token()).map(tok))
            ));
        }
        for row in 1..=t.rows(TableId::ManifestResource) {
            let r = md.get::<rows::ManifestResource>(row);
            line(format!(
                "RESOURCE {} offset={} flags={} name={} impl={}",
                tok(token(TableId::ManifestResource, row)),
                or_bad(r.clone().map(|r| r.offset)),
                or_bad(r.clone().map(|r| format!("{:08x}", r.flags))),
                or_bad(r.clone().and_then(|r| md.string(r.name))),
                or_bad(r.and_then(|r| r.implementation.token()).map(tok))
            ));
        }
        for row in 1..=t.rows(TableId::GenericParam) {
            let r = md.get::<rows::GenericParam>(row);
            line(format!(
                "GENERICPARAM {} owner={} number={} flags={} name={} constraints={}",
                tok(token(TableId::GenericParam, row)),
                or_bad(r.clone().and_then(|r| r.owner.token()).map(tok)),
                or_bad(r.clone().map(|r| r.number)),
                or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
                or_bad(r.and_then(|r| md.string(r.name))),
                tokens(
                    TableId::GenericParamConstraint,
                    md.generic_param_constraints(row)
                )
            ));
        }
        for row in 1..=t.rows(TableId::MethodSpec) {
            let r = md.get::<rows::MethodSpec>(row);
            let args = r.clone().and_then(|r| {
                let blob = md.blob(r.instantiation)?;
                self.types(&signature::decode_method_spec(&mut Bytes::new(blob))?)
            });
            line(format!(
                "METHODSPEC {} method={} args={}",
                tok(token(TableId::MethodSpec, row)),
                or_bad(r.and_then(|r| r.method.token()).map(tok)),
                or_bad(args)
            ));
        }
        for row in 1..=t.rows(TableId::GenericParamConstraint) {
            let r = md.get::<rows::GenericParamConstraint>(row);
            line(format!(
                "CONSTRAINT {} owner={} type={}",
                tok(token(TableId::GenericParamConstraint, row)),
                or_bad(
                    r.clone()
                        .map(|r| tok(token(TableId::GenericParam, r.owner)))
                ),
                or_bad(r.and_then(|r| r.constraint.token()).map(tok))
            ));
        }
        out
    }

    fn module(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Module>(row);
        let guid = |f: fn(&rows::Module) -> super::heaps::GuidIdx| {
            or_bad(
                r.clone()
                    .and_then(|r| md.guid(f(&r)))
                    .map(|g| format_guid(&g)),
            )
        };
        format!(
            "MODULE gen={} name={} mvid={} encid={} encbase={}",
            or_bad(r.clone().map(|r| r.generation)),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            guid(|r| r.mvid),
            guid(|r| r.enc_id),
            guid(|r| r.enc_base_id)
        )
    }

    fn type_ref(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::TypeRef>(row);
        format!(
            "TYPEREF {} scope={} name={} ns={}",
            tok(token(TableId::TypeRef, row)),
            or_bad(r.clone().and_then(|r| r.resolution_scope.token()).map(tok)),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            or_bad(r.and_then(|r| md.string(r.namespace)))
        )
    }

    fn type_def(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::TypeDef>(row);
        let list = |table, rows: Result<Vec<u32>>| or_bad(rows.map(|rows| tokens(table, rows)));
        format!(
            "TYPEDEF {} flags={} name={} ns={} extends={} decl={} layout={} fields={} methods={} events={} props={} ifaces={}",
            tok(token(TableId::TypeDef, row)),
            or_bad(r.clone().map(|r| format!("{:08x}", r.flags))),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            or_bad(r.clone().and_then(|r| md.string(r.namespace))),
            or_bad(r.and_then(|r| r.extends.token()).map(tok)),
            or_bad(
                md.declaring_type(row)
                    .map(|d| tok(token(TableId::TypeDef, d)))
            ),
            or_bad(
                md.type_layout(row)
                    .map(|(packing, size)| format!("{packing},{size}"))
            ),
            list(TableId::Field, md.type_fields(row)),
            list(TableId::MethodDef, md.type_methods(row)),
            list(TableId::Event, md.type_events(row)),
            list(TableId::Property, md.type_properties(row)),
            tokens(TableId::InterfaceImpl, md.type_interface_impls(row))
        )
    }

    fn field(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Field>(row);
        let sig = r.clone().and_then(|r| {
            self.ty(
                &signature::decode_field(&mut Bytes::new(md.blob(r.signature)?))?,
                0,
            )
        });
        format!(
            "FIELD {} flags={} name={} sig={} const={} rva={} offset={} marshal={}",
            tok(token(TableId::Field, row)),
            or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
            or_bad(r.and_then(|r| md.string(r.name))),
            or_bad(sig),
            tok(token(
                TableId::Constant,
                md.default_value(TableId::Field, row)
            )),
            or_bad(md.field_rva(row)),
            or_bad(md.field_offset(row)),
            or_bad(md.marshalling(TableId::Field, row).map(hex))
        )
    }

    fn method(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::MethodDef>(row);
        let sig = r.clone().and_then(|r| {
            self.method_sig(&signature::decode_method(&mut Bytes::new(
                md.blob(r.signature)?,
            ))?)
        });
        let import = match md.method_import(row) {
            0 => Ok(format!("0000,,{}", tok(token(TableId::ModuleRef, 0)))),
            map => md.get::<rows::ImplMap>(map).and_then(|m| {
                Ok(format!(
                    "{:04x},{},{}",
                    m.flags,
                    md.string(m.import_name)?,
                    tok(token(TableId::ModuleRef, m.import_scope))
                ))
            }),
        };
        format!(
            "METHOD {} rva={} impl={} flags={} name={} sig={} params={} import={}",
            tok(token(TableId::MethodDef, row)),
            or_bad(r.clone().map(|r| r.rva)),
            or_bad(r.clone().map(|r| format!("{:04x}", r.impl_flags))),
            or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
            or_bad(r.and_then(|r| md.string(r.name))),
            or_bad(sig),
            or_bad(md.method_params(row).map(|p| tokens(TableId::Param, p))),
            or_bad(import)
        )
    }

    fn body(&self, row: u32) -> Vec<String> {
        let md = self.md;
        let rva = match md.get::<rows::MethodDef>(row) {
            Ok(r) if r.rva != 0 => r.rva,
            _ => return Vec::new(),
        };
        let t = tok(token(TableId::MethodDef, row));
        match md.method_body(rva) {
            Err(_) => vec![format!("BODY {t} !bad")],
            Ok(body) => {
                let mut lines = vec![format!(
                    "BODY {t} maxstack={} locals={} init={} size={} il={}:{:016x}",
                    body.max_stack,
                    tok(token(TableId::StandAloneSig, body.local_sig)),
                    u8::from(body.init_locals),
                    body.size,
                    body.code.len(),
                    fnv1a(body.code)
                )];
                for c in &body.clauses {
                    let catch = if c.kind == 0 {
                        c.class_token_or_filter
                    } else {
                        0
                    };
                    let filter = if c.kind == 1 {
                        i64::from(c.class_token_or_filter as i32)
                    } else {
                        -1
                    };
                    lines.push(format!(
                        "EH {t} kind={} try={},{} handler={},{} catch={:08x} filter={filter}",
                        c.kind,
                        c.try_offset,
                        c.try_length,
                        c.handler_offset,
                        c.handler_length,
                        catch
                    ));
                }
                lines
            }
        }
    }

    fn param(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Param>(row);
        format!(
            "PARAM {} flags={} seq={} name={} const={} marshal={}",
            tok(token(TableId::Param, row)),
            or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
            or_bad(r.clone().map(|r| r.sequence)),
            or_bad(r.and_then(|r| md.string(r.name))),
            tok(token(
                TableId::Constant,
                md.default_value(TableId::Param, row)
            )),
            or_bad(md.marshalling(TableId::Param, row).map(hex))
        )
    }

    fn member_ref(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::MemberRef>(row);
        let sig = r.clone().and_then(|r| {
            let blob = md.blob(r.signature)?;
            let header = *blob.first().ok_or(Error::Truncated)?;
            let mut b = Bytes::new(blob);
            match signature::header_kind(header) {
                signature::HEADER_KIND_FIELD => Ok(format!(
                    "field {}",
                    self.ty(&signature::decode_field(&mut b)?, 0)?
                )),
                signature::HEADER_KIND_METHOD => {
                    self.method_sig(&signature::decode_method(&mut b)?)
                }
                _ => Err(Error::Malformed("member reference signature kind")),
            }
        });
        format!(
            "MEMBERREF {} parent={} name={} sig={}",
            tok(token(TableId::MemberRef, row)),
            or_bad(r.clone().and_then(|r| r.class.token()).map(tok)),
            or_bad(r.and_then(|r| md.string(r.name))),
            or_bad(sig)
        )
    }

    /// `(semantics flag, method row)` pairs of an Event or Property, in table order.
    fn accessors(&self, table: TableId, row: u32) -> Vec<(u16, u32)> {
        self.md
            .semantics(table, row)
            .filter_map(|r| self.md.get::<rows::MethodSemantics>(r).ok())
            .map(|s| (s.semantics, s.method))
            .collect()
    }

    fn event(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Event>(row);
        let (mut add, mut remove, mut raise, mut other) = (0, 0, 0, Vec::new());
        for (semantics, method) in self.accessors(TableId::Event, row) {
            match semantics {
                0x08 => add = method,
                0x10 => remove = method,
                0x20 => raise = method,
                0x04 => other.push(method),
                _ => {}
            }
        }
        let m = |r| tok(token(TableId::MethodDef, r));
        format!(
            "EVENT {} flags={} name={} type={} add={} remove={} raise={} other={}",
            tok(token(TableId::Event, row)),
            or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            or_bad(r.and_then(|r| r.event_type.token()).map(tok)),
            m(add),
            m(remove),
            m(raise),
            tokens(TableId::MethodDef, other)
        )
    }

    fn property(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Property>(row);
        let (mut get, mut set, mut other) = (0, 0, Vec::new());
        for (semantics, method) in self.accessors(TableId::Property, row) {
            match semantics {
                0x02 => get = method,
                0x01 => set = method,
                0x04 => other.push(method),
                _ => {}
            }
        }
        let sig = r.clone().and_then(|r| {
            self.method_sig(&signature::decode_method(&mut Bytes::new(
                md.blob(r.signature)?,
            ))?)
        });
        let m = |r| tok(token(TableId::MethodDef, r));
        format!(
            "PROPERTY {} flags={} name={} sig={} const={} get={} set={} other={}",
            tok(token(TableId::Property, row)),
            or_bad(r.clone().map(|r| format!("{:04x}", r.flags))),
            or_bad(r.and_then(|r| md.string(r.name))),
            or_bad(sig),
            tok(token(
                TableId::Constant,
                md.default_value(TableId::Property, row)
            )),
            m(get),
            m(set),
            tokens(TableId::MethodDef, other)
        )
    }

    fn assembly(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::Assembly>(row);
        format!(
            "ASSEMBLY hash={} version={} flags={} key={} name={} culture={}",
            or_bad(r.clone().map(|r| format!("{:08x}", r.hash_alg_id))),
            or_bad(
                r.clone()
                    .map(|r| format!("{}.{}.{}.{}", r.major, r.minor, r.build, r.revision))
            ),
            or_bad(r.clone().map(|r| format!("{:08x}", r.flags))),
            or_bad(r.clone().and_then(|r| md.blob(r.public_key)).map(hex)),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            or_bad(r.and_then(|r| md.string(r.culture)))
        )
    }

    fn assembly_ref(&self, row: u32) -> String {
        let md = self.md;
        let r = md.get::<rows::AssemblyRef>(row);
        format!(
            "ASSEMBLYREF {} version={} flags={} key={} name={} culture={} hash={}",
            tok(token(TableId::AssemblyRef, row)),
            or_bad(
                r.clone()
                    .map(|r| format!("{}.{}.{}.{}", r.major, r.minor, r.build, r.revision))
            ),
            or_bad(r.clone().map(|r| format!("{:08x}", r.flags))),
            or_bad(
                r.clone()
                    .and_then(|r| md.blob(r.public_key_or_token))
                    .map(hex)
            ),
            or_bad(r.clone().and_then(|r| md.string(r.name))),
            or_bad(r.clone().and_then(|r| md.string(r.culture))),
            or_bad(r.and_then(|r| md.blob(r.hash_value)).map(hex))
        )
    }

    fn types(&self, types: &[SigType]) -> Result<String> {
        let list = types
            .iter()
            .map(|t| self.ty(t, 0))
            .collect::<Result<Vec<_>>>()?;
        Ok(list.join(","))
    }

    fn method_sig(&self, sig: &MethodSig) -> Result<String> {
        let mut params = Vec::with_capacity(sig.params.len() + 1);
        for (i, p) in sig.params.iter().enumerate() {
            if i == sig.required_params {
                params.push("...".to_owned());
            }
            params.push(self.ty(p, 0)?);
        }
        Ok(format!(
            "hdr={:02x} gen={} {}({})",
            sig.header,
            sig.generic_params,
            self.ty(&sig.ret, 0)?,
            params.join(",")
        ))
    }

    fn ty(&self, ty: &SigType, depth: u32) -> Result<String> {
        Ok(match ty {
            SigType::Primitive(p) => format!("System.{}", p.name()),
            SigType::Handle { token: t, kind } => {
                let prefix = if *kind == HandleKind::ValueType {
                    "valuetype "
                } else {
                    ""
                };
                let row = t & 0x00FF_FFFF;
                let name = match t >> 24 {
                    0x02 => self.type_def_name(row, depth)?,
                    0x01 => self.type_ref_name(row, depth)?,
                    _ => {
                        let spec = self.md.get::<rows::TypeSpec>(row)?;
                        let blob = self.md.blob(spec.signature)?;
                        self.ty(
                            &signature::decode_type(&mut Bytes::new(blob))?,
                            self.deeper(depth)?,
                        )?
                    }
                };
                format!("{prefix}{name}")
            }
            SigType::GenericInst { generic, args } => {
                let args = args
                    .iter()
                    .map(|a| self.ty(a, depth))
                    .collect::<Result<Vec<_>>>()?;
                format!("{}<{}>", self.ty(generic, depth)?, args.join(","))
            }
            SigType::TypeParam(i) => format!("!{i}"),
            SigType::MethodParam(i) => format!("!!{i}"),
            SigType::Pointer(t) => format!("{}*", self.ty(t, depth)?),
            SigType::ByRef(t) => format!("{}&", self.ty(t, depth)?),
            SigType::Pinned(t) => format!("{} pinned", self.ty(t, depth)?),
            SigType::SzArray(t) => format!("{}[]", self.ty(t, depth)?),
            SigType::Array { elem, shape } => {
                let join = |v: Vec<String>| v.join(",");
                format!(
                    "{}[{};{};{}]",
                    self.ty(elem, depth)?,
                    shape.rank,
                    join(shape.sizes.iter().map(u32::to_string).collect()),
                    join(shape.lower_bounds.iter().map(i32::to_string).collect())
                )
            }
            SigType::FnPtr(sig) => format!("method {}", self.method_sig(sig)?),
            SigType::Modified {
                modifier,
                inner,
                required,
            } => format!(
                "{} {}({})",
                self.ty(inner, depth)?,
                if *required { "modreq" } else { "modopt" },
                self.ty(modifier, depth)?
            ),
        })
    }

    fn deeper(&self, depth: u32) -> Result<u32> {
        if depth >= MAX_NAME_DEPTH {
            Err(Error::Malformed("type reference cycle"))
        } else {
            Ok(depth + 1)
        }
    }

    fn qualified(&self, ns: super::heaps::StrIdx, name: super::heaps::StrIdx) -> Result<String> {
        let (ns, name) = (self.md.string(ns)?, self.md.string(name)?);
        Ok(if ns.is_empty() {
            name.into_owned()
        } else {
            format!("{ns}.{name}")
        })
    }

    fn type_def_name(&self, row: u32, depth: u32) -> Result<String> {
        let t = self.md.get::<rows::TypeDef>(row)?;
        let name = self.qualified(t.namespace, t.name)?;
        Ok(match self.md.declaring_type(row)? {
            0 => name,
            outer => format!("{}/{name}", self.type_def_name(outer, self.deeper(depth)?)?),
        })
    }

    fn type_ref_name(&self, row: u32, depth: u32) -> Result<String> {
        let t = self.md.get::<rows::TypeRef>(row)?;
        let name = self.qualified(t.namespace, t.name)?;
        Ok(match t.resolution_scope.decode()? {
            (TableId::TypeRef, outer) if outer != 0 => {
                format!("{}/{name}", self.type_ref_name(outer, self.deeper(depth)?)?)
            }
            _ => name,
        })
    }
}
