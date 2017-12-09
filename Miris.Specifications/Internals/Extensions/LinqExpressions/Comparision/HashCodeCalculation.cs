// Atualização do código @ https://github.com/lytico/db4o/blob/11fa71fc00feff0ef3a4fd1e39ed01e00c9311bc/db4o.net/Db4objects.Db4o.Linq/Db4objects.Db4o.Linq/Expressions/HashCodeCalculation.cs

using System.Linq.Expressions;

namespace Miris.Specifications.Internals.LinqExpressions.Comparision
{
    internal class HashCodeCalculation : ExpressionVisitor
    {
        public HashCodeCalculation(Expression expression)
        {
            Visit(expression);
        }

        public int HashCode { get; private set; }

        private void Add(int i)
        {
            HashCode *= 37;
            HashCode ^= i;
        }

        #region ' overrides '

        public sealed override Expression Visit(Expression expression)
        {
            if (expression == null) return null;

            Add((int) expression.NodeType);
            Add(expression.Type.GetHashCode());

            return base.Visit(expression);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            Add(node.Initializers.Count);
            return base.VisitListInit(node);
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value != null) Add(constant.Value.GetHashCode());
            return base.VisitConstant(constant);
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            Add(member.Member.GetHashCode());
            return base.VisitMember(member);
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            Add(methodCall.Method.GetHashCode());
            return base.VisitMethodCall(methodCall);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            Add(parameter.Name.GetHashCode());
            return base.VisitParameter(parameter);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression type)
        {
            Add(type.TypeOperand.GetHashCode());
            return base.VisitTypeBinary(type);
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            if (binary.Method != null) Add(binary.Method.GetHashCode());
            if (binary.IsLifted) Add(1);
            if (binary.IsLiftedToNull) Add(1);

            return base.VisitBinary(binary);
        }

        protected override Expression VisitUnary(UnaryExpression unary)
        {
            if (unary.Method != null) Add(unary.Method.GetHashCode());
            if (unary.IsLifted) Add(1);
            if (unary.IsLiftedToNull) Add(1);

            return base.VisitUnary(unary);
        }

        protected override Expression VisitNew(NewExpression nex)
        {
            Add(nex.Constructor.GetHashCode());
            Add(nex.Members.Count);
            return base.VisitNew(nex);
        }

        #endregion
    }
}