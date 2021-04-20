using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Com.Atomatus.Bootstarter
{
    internal abstract class DynamicType
    {
        protected class Key : IEqualityComparer<Key>
        {
            private readonly Guid guid;
            private readonly int hashCode;

            public Key(params Type[] args) : this(args.Select(t => t.GUID).ToArray()) { }
            
            public Key(IEnumerable<Type> args) : this(args.Select(t => t.GUID).ToArray()) { }

            public Key(params Guid[] args)
            {
                this.hashCode = Objects.GetHashCode(args);
                this.guid = args.Aggregate(Merge);
            }

            private Guid Merge(Guid curr, Guid next)
            {
                byte[] cArr = curr.ToByteArray();
                byte[] nArr = next.ToByteArray();

                for (int i = 0, c = 16 /*Guid len*/; i < c; i++)
                {
                    cArr[i] ^= nArr[i];
                }

                return new Guid(cArr);
            }

            public bool Equals(Key x, Key y) => x == y || x.hashCode == y.hashCode;

            public int GetHashCode(Key obj) => obj.hashCode;

            public override int GetHashCode() => hashCode;

            public override bool Equals(object obj) => obj is Key other && Equals(this, other);

            public override string ToString() => guid.ToString();
        }

        private readonly ConcurrentDictionary<Key, Type> dictionary;

        protected DynamicType()
        {
            dictionary = new ConcurrentDictionary<Key, Type>();
        }

        protected Type GetOrAdd([NotNull] Key key, [NotNull] Func<Key, Type> valueFactory)
        {
            return dictionary.GetOrAdd(key, valueFactory);
        }

        protected AssemblyName GetAssemblyName(Type type)
        {
            AssemblyName aname = new AssemblyName(type.GUID.ToString());
            aname.SetPublicKey(type.Assembly.GetName().GetPublicKey());
            return aname;
        }

        protected ModuleBuilder GetModuleBuilder(AssemblyName aname, Key key, string moduleName = null)
        {
            return AssemblyBuilder
                   .DefineDynamicAssembly(aname, AssemblyBuilderAccess.Run)
                   .DefineDynamicModule(string.IsNullOrWhiteSpace(moduleName) ? 
                        key.ToString() : $"{key}.{moduleName}");
        }

        protected TypeBuilder GetTypeBuilder(ModuleBuilder builder, string name, 
            Type parent, 
            TypeAttributes attr = TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
            params Type[] interfaces)
        {
            return builder.DefineType(name, attr, parent, interfaces);
        }

        protected void SetProperty(TypeBuilder typeBuilder, Type pType, string name)
        {
            var fieldBuilder = typeBuilder.DefineField(
                fieldName: "_" + name, 
                type: pType, 
                attributes: FieldAttributes.Private);

            var propBuilder = typeBuilder.DefineProperty(
                    name: name,
                    attributes: PropertyAttributes.None,
                    callingConvention: CallingConventions.HasThis,
                    returnType: pType,
                    parameterTypes: null);

            var getSetAttr = MethodAttributes.Public | 
                MethodAttributes.HideBySig | 
                MethodAttributes.Virtual;

            //build getter
            MethodBuilder getter = typeBuilder.DefineMethod("get_" + name, getSetAttr, pType, Type.EmptyTypes);
            ILGenerator getterILG = getter.GetILGenerator();
            getterILG.Emit(OpCodes.Ldarg_0);
            getterILG.Emit(OpCodes.Ldfld, fieldBuilder);
            getterILG.Emit(OpCodes.Ret);
            propBuilder.SetGetMethod(getter);

            //build setter
            MethodBuilder setter = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { pType });
            ILGenerator setterILG = setter.GetILGenerator();
            setterILG.Emit(OpCodes.Ldarg_0);
            setterILG.Emit(OpCodes.Ldarg_1);
            setterILG.Emit(OpCodes.Stfld, fieldBuilder);
            setterILG.Emit(OpCodes.Ret);
            propBuilder.SetSetMethod(setter);
        }

        protected void SetConstructorSingleArgumentAndCallBase(TypeBuilder typeBuilder, Type parent,
            params Type[] paramType)
        {
            //parent constructor with same parameters.
            var ctorParent = parent.GetConstructor(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance, null, 
                paramType, null);

            //create a constructor from base type
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard | CallingConventions.HasThis,
                paramType);

            //set constructor operation to call base type constructor
            var ilGenerator = ctorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0); // push "this"
            ilGenerator.Emit(OpCodes.Ldarg_1); // push the DbContextOptions parameter to base type
            ilGenerator.Emit(OpCodes.Call, ctorParent); // call base constructor
            ilGenerator.Emit(OpCodes.Nop); // C# compiler add 2 NOPS, so
            ilGenerator.Emit(OpCodes.Nop); // we'll add them, too.
            ilGenerator.Emit(OpCodes.Ret); //return
        }

    }

}
