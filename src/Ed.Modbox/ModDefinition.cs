using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpFileSystem;

namespace Ed.Modbox
{
    /// <summary>
    /// High-level definition of a mod, as used in the mod list and in the
    /// mod manifest for dependencies. Translated into Mods by IModLocators.
    /// </summary>
    public class ModDefinition : IEquatable<ModDefinition>
    {
        public readonly String Name;
        public readonly Int32 Major;
        public readonly Int32 Minor;
        public readonly ModMatchingRule MatchingRule;

        public ModDefinition(String name, Int32 major, 
                             Int32 minor, ModMatchingRule matchingRule)
        {
            Name = name;
            Major = major;
            Minor = minor;
            MatchingRule = matchingRule;
        }

        /// <summary>
        /// Determines if another ModDefinition is of a compatible value to this
        /// one such that its conditions are satisfied. Names must match exactly
        /// to satisfy, as must major versions in all cases; minor versions must
        /// match according to the MatchingRule _of the one being called_. As
        /// such, this method is not transitive; A.IsSatisfiedBy(B) may not be
        /// equal to B.IsSatisfiedBy(A).
        /// </summary>
        /// <param name="other">The other ModDefinition to match against.</param>
        /// <returns>
        /// True if the other mod definition satisfies this one's requirements.
        /// </returns>
        public Boolean IsSatisfiedBy(ModDefinition other)
        {
            if (this.Name != other.Name) return false;
            if (this.Major != other.Major) return false;

            switch (this.MatchingRule)
            {
                case ModMatchingRule.AnyMinor:
                    return true;
                case ModMatchingRule.Exact:
                    return this.Minor == other.Minor;
                case ModMatchingRule.MinorOrGreater:
                    return this.Minor <= other.Minor;
                default:
                    throw new Exception("impossible");
            }
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ModDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Major == Major && other.Minor == Minor && Equals(other.MatchingRule, MatchingRule);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ModDefinition)) return false;
            return Equals((ModDefinition) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Major;
                result = (result*397) ^ Minor;
                result = (result*397) ^ MatchingRule.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(ModDefinition left, ModDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ModDefinition left, ModDefinition right)
        {
            return !Equals(left, right);
        }


        /// <summary>
        /// Parses a mod definition from a string of the form:
        /// 
        /// modname-1.0
        /// 
        /// Does not support hyphens anywhere except between the mod name and
        /// the version. Supports all version formats described in the
        /// ModMatchingRule enum.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ModDefinition Parse(String input)
        {
            String[] tokens = input.Split('-');

            if (tokens.Length != 2)
            {
                throw new ParseException(String.Format("string '{0}': " +
                    "{1} tokens found, {2} expected", input, tokens.Length, 2));
            }

            String name = tokens[0];
            String version = tokens[1];
            ModMatchingRule matchingRule = ModMatchingRule.Exact;

            String[] versionTokens = version.Split('.');
            if (versionTokens.Length != 2)
            {
                throw new ParseException(String.Format("version string '{0}': " +
                    "{1} tokens found, {2} expected", version, versionTokens.Length, 2));
            }

            Int32 major = Int32.Parse(versionTokens[0]);
            Int32 minor = 0;

            if (version.EndsWith("x")) // any minor
            {
                matchingRule = ModMatchingRule.AnyMinor;
            }
            else if (version.EndsWith("+")) // minor or greater
            {
                matchingRule = ModMatchingRule.MinorOrGreater;
                minor = Int32.Parse(versionTokens[1].TrimEnd('+'));
            }
            else // exact
            {
                minor = Int32.Parse(versionTokens[1]);
            }

            return new ModDefinition(name, major, minor, matchingRule);
        }
    }

    /// <summary>
    /// Definition of matching type for mod definitions.
    /// </summary>
    public enum ModMatchingRule
    {
        /// <summary>
        /// Only this exact version of the mod will work. Defined by
        /// a version string of the form "1.0".
        /// </summary>
        Exact,

        /// <summary>
        /// Any minor version of the given major version will work. Defined by
        /// a version string of the form "2.x", which will match "2.0", "2.2",
        /// and so on.
        /// </summary>
        AnyMinor,

        /// <summary>
        /// This minor version, or any later minor version of this major
        /// version, will work. Defined by a version string of the form "1.5+",
        /// which will work for "1.5", "1.7", etc.
        /// </summary>
        MinorOrGreater
    }
}
