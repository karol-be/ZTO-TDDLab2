using System.Collections.Generic;

namespace BasicUtils
{
    public interface IRule
    {
        // Properties
        string Description { get; }
        string Name { get; }
    }



    public interface IValidatedObject
    {
        // Methods
        IEnumerable<IRule> Validate();

        // Properties
        bool IsValid { get; }
    }

    public interface IBusinessRule<T> : IRule
    {
        // Methods
        bool IsSatisfiedBy(T item);
    }

 
    public interface IBusinessRuleSet
    {
        // Methods
        IEnumerable<IRule> BrokenBy(IValidatedObject item);
        bool Contains(IRule rule);

        // Properties
        int Count { get; }
        bool IsEmpty { get; }
        IList<string> Messages { get; }
    }
}
