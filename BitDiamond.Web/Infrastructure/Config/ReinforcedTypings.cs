using Axis.Pollux.Identity.Principal;
using System.Linq;

using Axis.Luna.Extensions;
using BitDiamond.Core.Models;
using Reinforced.Typings.Generators;
using Reinforced.Typings;
using Reinforced.Typings.Ast;
using System;
using Reinforced.Typings.Fluent;
using Axis.Pollux.Authentication;
using Axis.Pollux.RBAC.Auth;
using System.Collections.Generic;

namespace BitDiamond.Web.Infrastructure.Config
{
    /// <summary>
    /// This class is called by the ReinforcedTypings build extension everytime the project is built.
    /// </summary>
    public static class ReinforcedTypings
    {
        public static void Configure(ConfigurationBuilder config)
        {
            #region  pollux models

            var polluxBase = typeof(PolluxEntity<>);
            config.ExportAsInterfaces(polluxBase.Enumerate(), _icb =>
            {
                _icb.OverrideNamespace("Pollux.Models")
                    .WithCodeGenerator<PolluxModelGenerator>();
            });

            #region Identity

            //enum
            polluxBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsEnums(_tarr, _icb =>
                {
                    _icb.OverrideNamespace("Pollux.Models");
                }));

            //models
            config.ExportAsInterfaces(typeof(CredentialMetadata).Enumerate(), _icb =>
            {
                _icb.OverrideNamespace("Pollux.Models")
                    .WithCodeGenerator<PolluxModelGenerator>();
            });
            polluxBase
                .Assembly
                .GetTypes()
                .Where(_t => _t != polluxBase)
                .Where(_t => _t.HasGenericAncestor(polluxBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithCodeGenerator<PolluxModelGenerator>()
                        .OverrideNamespace("Pollux.Models");
                }));


            #endregion

            #region Authentication

            //enum
            typeof(Credential)
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsEnums(_tarr, _icb =>
                {
                    _icb.OverrideNamespace("Pollux.Models");
                }));

            //models
            typeof(Credential)
                .Assembly
                .GetTypes()
                .Where(_t => _t.HasGenericAncestor(polluxBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithCodeGenerator<PolluxModelGenerator>()
                        .OverrideNamespace("Pollux.Models");
                }));

            #endregion

            #region RBAC

            //enum
            typeof(Role)
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsEnums(_tarr, _icb =>
                {
                    _icb.OverrideNamespace("Pollux.Models");
                }));

            //models
            typeof(Role)
                .Assembly
                .GetTypes()
                .Where(_t => _t.HasGenericAncestor(polluxBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithCodeGenerator<PolluxModelGenerator>()
                        .OverrideNamespace("Pollux.Models");
                }));

            //pocos
            polluxBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.Namespace.StartsWith("Axis.Pollux.RBAC.Auth"))
                .Where(_t => _t.IsClass)
                .Where(_t => _t != polluxBase)
                .Where(_t => !_t.HasGenericAncestor(polluxBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithCodeGenerator<PolluxModelGenerator>()
                        .OverrideNamespace("Pollux.Models");
                }));

            #endregion

            #endregion


            #region BitDiamond models

            var bitDiamondBase = typeof(BaseModel<>);
            config.ExportAsInterfaces(bitDiamondBase.Enumerate(), _icb =>
            {
                _icb.OverrideNamespace("BitDiamond.Models")
                    .WithPublicProperties();
            });

            //enums
            bitDiamondBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.IsEnum)
                .Do(_tarr => config.ExportAsEnums(_tarr, _icb =>
                {
                    _icb.OverrideNamespace("BitDiamond.Models");
                }));

            //models
            bitDiamondBase
                .Assembly
                .GetTypes()
                .Where(_t => _t.HasGenericAncestor(bitDiamondBase))
                .Do(_tarr => config.ExportAsInterfaces(_tarr, _icb =>
                {
                    _icb.WithCodeGenerator<BitDiamondModelGenerator>()
                        .OverrideNamespace("BitDiamond.Models");
                }));

            #endregion
        }


        public static string ToTypeScriptType(Type t)
        {
            if (t == typeof(string)) return "string";
            else if (t.IsNumeric()) return "number";
            else if (t == typeof(bool)) return "boolean";
            else if (t.IsArray) return $"{ToTypeScriptType(t.GetElementType())}[]";
            else if (t == typeof(DateTime) || t == typeof(DateTime?)) return "Apollo.Models.JsonDateTime";
            else if (t == typeof(TimeSpan) || t == typeof(TimeSpan?)) return "Apollo.Models.JsonTimeSpan";
            else return "any";
        }
    }

    public class PolluxModelGenerator : InterfaceCodeGenerator
    {
        public override RtInterface GenerateNode(Type element, RtInterface node, TypeResolver resolver)
        {
            var n = node;
            if (n == null) return null;
            else n = new RtInterface
            {
                Name = new RtSimpleTypeName
                (
                    $"I{(element.IsGenericType ? element.Name.Substring(0, element.Name.IndexOf("`")) : element.Name)}",
                    element.IsGenericType ? element.GetGenericArguments().Select(_garg => new RtSimpleTypeName(_garg.Name)).ToArray() : new RtSimpleTypeName[0]
                )
            };

            foreach (var m in element.GetProperties().Where(_p => _p.DeclaringType == element))
            {
                var generator = resolver.GeneratorFor(m, Context);
                var member = generator.Generate(m, resolver);

                if (m.PropertyType == typeof(DateTime) || m.PropertyType == typeof(DateTime?))
                    member.As<RtField>().Type = new RtSimpleTypeName("Apollo.Models.JsonDateTime");

                else if (m.PropertyType == typeof(TimeSpan) || m.PropertyType == typeof(TimeSpan?))
                    member.As<RtField>().Type = new RtSimpleTypeName(new RtTypeName[0], "Apollo.Models", "JsonDateTime");

                else if (typeof(IEnumerable<byte>).IsAssignableFrom(m.PropertyType))
                    member.As<RtField>().Type = new RtSimpleTypeName("string");

                n.Members.Add(member);
            }

            if (element.HasGenericAncestor(typeof(PolluxEntity<>)))
                n.Implementees.Add(new RtSimpleTypeName("IPolluxEntity", new[] { new RtSimpleTypeName(ReinforcedTypings.ToTypeScriptType(element.BaseType.GetGenericArguments()[0])) }));

            return n;
        }
    }

    public class BitDiamondModelGenerator : InterfaceCodeGenerator
    {
        public override RtInterface GenerateNode(Type element, RtInterface node, TypeResolver resolver)
        {
            var n = node;
            if (n == null) return null;
            else n = new RtInterface
            {
                Name = new RtSimpleTypeName
                (
                    $"I{(element.IsGenericType ? element.Name.Substring(0, element.Name.IndexOf("`")) : element.Name)}",
                    element.IsGenericType ? element.GetGenericArguments().Select(_garg => new RtSimpleTypeName(_garg.Name)).ToArray() : new RtSimpleTypeName[0]
                )
            };

            foreach (var m in element.GetProperties().Where(_p => _p.DeclaringType == element))
            {
                var generator = resolver.GeneratorFor(m, Context);
                var member = generator.Generate(m, resolver);

                if (m.PropertyType == typeof(DateTime) || m.PropertyType == typeof(DateTime?))
                    member.As<RtField>().Type = new RtSimpleTypeName("Apollo.Models.JsonDateTime");

                else if (m.PropertyType == typeof(TimeSpan) || m.PropertyType == typeof(TimeSpan?))
                    member.As<RtField>().Type = new RtSimpleTypeName(new RtTypeName[0], "Apollo.Models", "JsonDateTime");

                else if(typeof(IEnumerable<byte>).IsAssignableFrom(m.PropertyType))
                    member.As<RtField>().Type = new RtSimpleTypeName("string");

                n.Members.Add(member);
            }

            if (!element.IsGenericType)
                n.Implementees.Add(new RtSimpleTypeName("IBaseModel", new[] { new RtSimpleTypeName(ReinforcedTypings.ToTypeScriptType(element.BaseType.GetGenericArguments()[0])) }));

            return n;
        }
    }
}