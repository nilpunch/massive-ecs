// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace System.Diagnostics.CodeAnalysis
{
#if !NET5_0_OR_GREATER
	/// <summary>Indicates that certain members on a specified <see cref="T:System.Type" /> are accessed dynamically, for example, through <see cref="N:System.Reflection" />.</summary>
	[AttributeUsage(
		AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter |
		AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
	public sealed class DynamicallyAccessedMembersAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute" /> class with the specified member types.</summary>
		/// <param name="memberTypes">The types of the dynamically accessed members.</param>
		public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
		{
			this.MemberTypes = memberTypes;
		}

		/// <summary>Gets the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes" /> that specifies the type of dynamically accessed members.</summary>
		public DynamicallyAccessedMemberTypes MemberTypes { get; }
	}

	/// <summary>Specifies the types of members that are dynamically accessed.
	/// This enumeration has a <see cref="T:System.FlagsAttribute" /> attribute that allows a bitwise combination of its member values.</summary>
	[Flags]
	public enum DynamicallyAccessedMemberTypes
	{
		/// <summary>Specifies no members.</summary>
		None = 0,

		/// <summary>Specifies the default, parameterless public constructor.</summary>
		PublicParameterlessConstructor = 1,

		/// <summary>Specifies all public constructors.</summary>
		PublicConstructors = 3,

		/// <summary>Specifies all non-public constructors.</summary>
		NonPublicConstructors = 4,

		/// <summary>Specifies all public methods.</summary>
		PublicMethods = 8,

		/// <summary>Specifies all non-public methods.</summary>
		NonPublicMethods = 16, // 0x00000010

		/// <summary>Specifies all public fields.</summary>
		PublicFields = 32, // 0x00000020

		/// <summary>Specifies all non-public fields.</summary>
		NonPublicFields = 64, // 0x00000040

		/// <summary>Specifies all public nested types.</summary>
		PublicNestedTypes = 128, // 0x00000080

		/// <summary>Specifies all non-public nested types.</summary>
		NonPublicNestedTypes = 256, // 0x00000100

		/// <summary>Specifies all public properties.</summary>
		PublicProperties = 512, // 0x00000200

		/// <summary>Specifies all non-public properties.</summary>
		NonPublicProperties = 1024, // 0x00000400

		/// <summary>Specifies all public events.</summary>
		PublicEvents = 2048, // 0x00000800

		/// <summary>Specifies all non-public events.</summary>
		NonPublicEvents = 4096, // 0x00001000

		/// <summary>Specifies all interfaces implemented by the type.</summary>
		Interfaces = 8192, // 0x00002000

		/// <summary>Specifies all members.</summary>
		All = -1, // 0xFFFFFFFF
	}

	/// <summary>States a dependency that one member has on another.</summary>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
	public sealed class DynamicDependencyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute" /> class with the specified signature of a member on the same type as the consumer.</summary>
		/// <param name="memberSignature">The signature of the member depended on.</param>
		public DynamicDependencyAttribute(string memberSignature)
		{
			this.MemberSignature = memberSignature;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute" /> class with the specified signature of a member on a <see cref="T:System.Type" />.</summary>
		/// <param name="memberSignature">The signature of the member depended on.</param>
		/// <param name="type">The type that contains <paramref name="memberSignature" />.</param>
		public DynamicDependencyAttribute(string memberSignature, Type type)
		{
			this.MemberSignature = memberSignature;
			this.Type = type;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute" /> class with the specified signature of a member on a type in an assembly.</summary>
		/// <param name="memberSignature">The signature of the member depended on.</param>
		/// <param name="typeName">The full name of the type containing the specified member.</param>
		/// <param name="assemblyName">The assembly name of the type containing the specified member.</param>
		public DynamicDependencyAttribute(string memberSignature, string typeName, string assemblyName)
		{
			this.MemberSignature = memberSignature;
			this.TypeName = typeName;
			this.AssemblyName = assemblyName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute" /> class with the specified types of members on a <see cref="T:System.Type" />.</summary>
		/// <param name="memberTypes">The types of members depended on.</param>
		/// <param name="type">The type that contains the specified members.</param>
		public DynamicDependencyAttribute(DynamicallyAccessedMemberTypes memberTypes, Type type)
		{
			this.MemberTypes = memberTypes;
			this.Type = type;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute" /> class with the specified types of members on a type in an assembly.</summary>
		/// <param name="memberTypes">The types of members depended on.</param>
		/// <param name="typeName">The full name of the type containing the specified members.</param>
		/// <param name="assemblyName">The assembly name of the type containing the specified members.</param>
		public DynamicDependencyAttribute(
			DynamicallyAccessedMemberTypes memberTypes,
			string typeName,
			string assemblyName)
		{
			this.MemberTypes = memberTypes;
			this.TypeName = typeName;
			this.AssemblyName = assemblyName;
		}

		/// <summary>Gets the signature of the member depended on.</summary>
		public string? MemberSignature { get; }

		/// <summary>Gets the types of the members that are depended on, for example, fields and properties.</summary>
		public DynamicallyAccessedMemberTypes MemberTypes { get; }

		/// <summary>Gets the <see cref="T:System.Type" /> containing the specified member.</summary>
		public Type? Type { get; }

		/// <summary>Gets the full name of the type containing the specified member.</summary>
		public string? TypeName { get; }

		/// <summary>Gets the assembly name of the specified type.</summary>
		public string? AssemblyName { get; }

		/// <summary>Gets or sets the condition in which the dependency is applicable.</summary>
		public string? Condition { get; set; }
	}

	/// <summary>Suppresses reporting of a specific rule violation, allowing multiple suppressions on a single code artifact.</summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	public sealed class UnconditionalSuppressMessageAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessageAttribute" /> class, specifying the category of the tool and the identifier for an analysis rule.</summary>
		/// <param name="category">The category for the attribute.</param>
		/// <param name="checkId">The identifier of the analysis rule the attribute applies to.</param>
		public UnconditionalSuppressMessageAttribute(string category, string checkId)
		{
			this.Category = category;
			this.CheckId = checkId;
		}

		/// <summary>Gets the category identifying the classification of the attribute.</summary>
		public string Category { get; }

		/// <summary>Gets the identifier of the analysis tool rule to be suppressed.</summary>
		public string CheckId { get; }

		/// <summary>Gets or sets the scope of the code that is relevant for the attribute.</summary>
		public string? Scope { get; set; }

		/// <summary>Gets or sets a fully qualified path that represents the target of the attribute.</summary>
		public string? Target { get; set; }

		/// <summary>Gets or sets an optional argument expanding on exclusion criteria.</summary>
		public string? MessageId { get; set; }

		/// <summary>Gets or sets the justification for suppressing the code analysis message.</summary>
		public string? Justification { get; set; }
	}
#endif
}
