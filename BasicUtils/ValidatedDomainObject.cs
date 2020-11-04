using System.Collections.Generic;

namespace BasicUtils
{
    public class ValidatedDomainObject : IValidatedObject
    {
        // Methods
        public IEnumerable<IRule> Validate()
        {
            return this.Rules.BrokenBy(this);
        }

        // Properties
        public bool IsValid
        {
            get
            {
                return Validate().IsEmpty();
            }
        }

        protected virtual IBusinessRuleSet Rules
        {
            get;set;
        }
    }
}