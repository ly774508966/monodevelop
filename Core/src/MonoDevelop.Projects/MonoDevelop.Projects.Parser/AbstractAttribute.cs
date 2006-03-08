// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections;
using System.CodeDom;
using MonoDevelop.Projects.Utility;

namespace MonoDevelop.Projects.Parser
{
	[Serializable]
	public class AbstractAttributeSection : IAttributeSection
	{
		protected AttributeTarget     attributeTarget;
		protected AttributeCollection attributes = new AttributeCollection();

		protected IRegion region;

		public virtual AttributeTarget AttributeTarget {
			get {
				return attributeTarget;
			}
			set {
				attributeTarget = value;
			}
		}

		public virtual AttributeCollection Attributes {
			get {
				return attributes;
			}
		}
		
		public IRegion Region
		{
			get { return region; }
		}
		
		public virtual int CompareTo(IAttributeSection value) {
			int cmp;
			
			if(0 != (cmp = (int)(AttributeTarget - value.AttributeTarget)))
				return cmp;
			
			return DiffUtility.Compare(Attributes, value.Attributes);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IAttributeSection)value);
		}
		
		public override bool Equals (object ob)
		{
			IAttributeSection sec = ob as IAttributeSection;
			if (sec == null) return false;
			return CompareTo (sec) == 0;
		}
		
		public override int GetHashCode ()
		{
			int c = 0;
			foreach (IAttribute at in Attributes)
				c += at.GetHashCode ();

			return attributeTarget.GetHashCode () + c;
		}
	}
	
	public abstract class AbstractAttribute : IAttribute
	{
		protected string name;
		protected CodeExpression[] positionalArguments;
		protected NamedAttributeArgument[] namedArguments;
		protected IRegion region;

		public virtual string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		public virtual CodeExpression[] PositionalArguments { // [expression]
			get {
				return positionalArguments;
			}
			set {
				positionalArguments = value;
			}
		}
		public virtual NamedAttributeArgument[] NamedArguments { // string/expression
			get {
				return namedArguments;
			}
			set {
				namedArguments = value;
			}
		}
		
		public IRegion Region
		{
			get { return region; }
		}
		
		public virtual int CompareTo(IAttribute value) {
			int cmp;
			
			cmp = Name.CompareTo(value.Name);
			if (cmp != 0) {
				return cmp;
			}
			
			cmp = DiffUtility.Compare(PositionalArguments, value.PositionalArguments);
			if (cmp != 0) {
				return cmp;
			}
			
			return DiffUtility.Compare(NamedArguments, value.NamedArguments);
		}
		
		int IComparable.CompareTo(object value) {
			return CompareTo((IAttribute)value);
		}
		
		public override bool Equals (object ob)
		{
			IAttribute other = ob as IAttribute;
			if (other == null) return false;
			return CompareTo (other) == 0;
		}
		
		public override int GetHashCode ()
		{
			return Name.GetHashCode ();
		}
	}
}
