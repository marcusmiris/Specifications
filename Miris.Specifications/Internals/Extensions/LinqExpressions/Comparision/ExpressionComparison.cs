// Atualização do código @ https://github.com/lytico/db4o/blob/11fa71fc00feff0ef3a4fd1e39ed01e00c9311bc/db4o.net/Db4objects.Db4o.Linq/Db4objects.Db4o.Linq/Expressions/ExpressionComparison.cs

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions.Comparision
{
    internal class ExpressionComparison : ExpressionVisitor
    {
        private readonly Queue<Expression> _candidates;
        private Expression _candidate;

        #region ' Constructor '

        public ExpressionComparison(Expression a, Expression b)
        {
            _candidates = new Queue<Expression>(new ExpressionEnumeration(b));

            Visit(a);

            if (_candidates.Count > 0) Stop();
        }

        #endregion

        public bool AreEqual { get; private set; } = true;

        private Expression PeekCandidate()
        {
            return _candidates.Count == 0 ? null : _candidates.Peek();
        }

        private Expression PopCandidate()
        {
            return _candidates.Dequeue();
        }

        private bool CheckAreOfSameType(Expression candidate, Expression expression)
        {
            return CheckEqual(expression.NodeType, candidate.NodeType)
                   && CheckEqual(expression.Type, candidate.Type);
        }

        private void Stop()
        {
            AreEqual = false;
        }

        private T CandidateFor<T>(T original) where T : Expression
        {
            return (T) _candidate;
        }

        private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates)
        {
            CompareList(collection, candidates,
                (item, candidate) => EqualityComparer<T>.Default.Equals(item, candidate));
        }

        private void CompareList<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidates,
            Func<T, T, bool> comparer)
        {
            if (!CheckAreOfSameSize(collection, candidates)) return;

            for (var i = 0; i < collection.Count; i++)
                if (!comparer(collection[i], candidates[i]))
                {
                    Stop();
                    return;
                }
        }

        private bool CheckAreOfSameSize<T>(ReadOnlyCollection<T> collection, ReadOnlyCollection<T> candidate)
        {
            return CheckEqual(collection.Count, candidate.Count);
        }

        private bool CheckNotNull<T>(T t) where T : class
        {
            if (t == null)
            {
                Stop();
                return false;
            }

            return true;
        }

        private bool CheckEqual<T>(T t, T candidate)
        {
            if (!EqualityComparer<T>.Default.Equals(t, candidate))
            {
                Stop();
                return false;
            }

            return true;
        }

        #region ' Overrides '

        public sealed override Expression Visit(Expression expression)
        {
            if (expression == null) return null;
            if (!AreEqual) return expression;

            _candidate = PeekCandidate();
            if (!CheckNotNull(_candidate)) return expression;
            if (!CheckAreOfSameType(_candidate, expression)) return expression;

            PopCandidate();

            return base.Visit(expression);
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            var candidate = CandidateFor(constant);
            CheckEqual(constant.Value, candidate.Value);
            return base.VisitConstant(constant);
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            var candidate = CandidateFor(member);
            CheckEqual(member.Member, candidate.Member);
            return base.VisitMember(member);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            var candidate = CandidateFor(methodCall);
            CheckEqual(methodCall.Method, candidate.Method);
            return base.VisitMethodCall(methodCall);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            var candidate = CandidateFor(parameter);
            CheckEqual(parameter.Type, candidate.Type);
            return base.VisitParameter(parameter);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression type)
        {
            var candidate = CandidateFor(type);
            CheckEqual(type.TypeOperand, candidate.TypeOperand);
            return base.VisitTypeBinary(type);
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            var candidate = CandidateFor(binary);
            if (!CheckEqual(binary.Method, candidate.Method)) return binary;
            if (!CheckEqual(binary.IsLifted, candidate.IsLifted)) return binary;
            if (!CheckEqual(binary.IsLiftedToNull, candidate.IsLiftedToNull)) return binary;
            return base.VisitBinary(binary);
        }

        protected override Expression VisitUnary(UnaryExpression unary)
        {
            var candidate = CandidateFor(unary);
            if (!CheckEqual(unary.Method, candidate.Method)) return unary;
            if (!CheckEqual(unary.IsLifted, candidate.IsLifted)) return unary;
            if (!CheckEqual(unary.IsLiftedToNull, candidate.IsLiftedToNull)) return unary;
            return base.VisitUnary(unary);
        }

        protected override Expression VisitNew(NewExpression nex)
        {
            var candidate = CandidateFor(nex);
            if (!CheckEqual(nex.Constructor, candidate.Constructor)) return nex;
            CompareList(nex.Members, candidate.Members);
            return base.VisitNew(nex);
        }

        #endregion
    }
}