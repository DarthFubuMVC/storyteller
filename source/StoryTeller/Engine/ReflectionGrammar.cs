using System;
using System.Reflection;
using StoryTeller.Domain;
using StoryTeller.Engine.Reflection;

namespace StoryTeller.Engine
{
    public abstract class ReflectionGrammar : LineGrammar
    {
        private readonly object _target;
        private Action<object> _callback;
        protected MethodInfo _method;


        public ReflectionGrammar(MethodInfo method, object target)
            : base(method.GetTemplate())
        {
            _method = method;
            _target = target;
        }

        public override string Description { get { return DescriptionAttribute.GetDescription(_method); } }

        public override void Execute(IStep containerStep, ITestContext context)
        {
            _callback = value => _method.GetReturnCell().RecordActual(value, containerStep, context);

            _method.Call(_target, containerStep, context, _callback);
        }
    }
}