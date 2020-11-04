using System.Collections.Generic;

namespace BasicUtils
{
    public class BusinessRuleSet<T> : IBusinessRuleSet where T : IValidatedObject
    {
        // Fields
        private IList<IBusinessRule<T>> rules;

        // Methods
        public BusinessRuleSet(params IBusinessRule<T>[] rules)
            : this(new List<IBusinessRule<T>>(rules))
        {
        }

        public BusinessRuleSet(IList<IBusinessRule<T>> rules)
        {
            this.rules = rules;
        }

        public IEnumerable<IRule> BrokenBy(IValidatedObject item)
        {
            IList<IRule> list = new List<IRule>();
            foreach (IBusinessRule<T> rule in this.rules)
            {
                if (!rule.IsSatisfiedBy((T)item))
                {
                    list.Add(rule);
                }
            }
            return list;
        }

        public bool Contains(IRule rule)
        {
            foreach (IBusinessRule<T> rule2 in this.rules)
            {
                if (rule.Name.Equals(rule2.Name))
                {
                    return true;
                }
            }
            return false;
        }

        // Properties
        public int Count
        {
            get
            {
                return this.rules.Count;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (this.Count == 0);
            }
        }

        public IList<string> Messages
        {
            get
            {
                return new List<IBusinessRule<T>>(this.rules).ConvertAll<string>(delegate(IBusinessRule<T> rule)
                                                                                     {
                                                                                         return rule.Description;
                                                                                     });
            }
        }
    }
}