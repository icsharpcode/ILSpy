using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.Decompiler.Ast.Transforms
{
    public class VariableDetupler : ContextTrackingVisitor<object>
    {
        public VariableDetupler(DecompilerContext context) : base(context)
        {
        }



        public override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data)
        {
            var identifier = assignmentExpression.Left as IdentifierExpression;
            var rightIdentifier = assignmentExpression.Right as IdentifierExpression;
            if (identifier != null && rightIdentifier != null && FindAssignment(identifier.IdentifierToken) == assignmentExpression) {
                var firstAssignment = FindAssignment(rightIdentifier.IdentifierToken);
                if(firstAssignment != null) {
                    ReplaceVar(identifier.IdentifierToken, rightIdentifier);
                    RemoveAssignment(assignmentExpression);
                }
            }
            return base.VisitAssignmentExpression(assignmentExpression, data);
        }

        private void ReplaceVar(Identifier from, IdentifierExpression to)
        {
            var block = from.Ancestors.OfType<BlockStatement>().First();

            foreach (var d in block.Descendants.OfType<IdentifierExpression>()) {
                if (d.Identifier == from.Name) {
                    d.ReplaceWith(to.Clone());
                }
            }
        }

        private void RemoveAssignment(AssignmentExpression expression)
        {
            if(expression.Parent is ExpressionStatement) {
                expression.Parent.Remove();
            }
            else {
                expression.ReplaceWith(expression.Right);
            }
        }

        //protected bool IsReadOnlyVariable(BlockStatement block, string name, INode allowedAssign)
        //{
        //    foreach (var identExpr in block.Descendants.OfType<IdentifierExpression>()) {
        //        if (identExpr.Identifier == name && identExpr != allowedAssign) {
        //            if (!(identExpr.Parent is MemberReferenceExpression && identExpr.Parent.Annotation<FieldReference>() != null)) {
        //                var assign = identExpr.Parent as AssignmentExpression;
        //                if (assign == null || assign. || !IsReadOnlyVariable(block, assign.Left.) {

        //                }
        //                return false;
        //            }
        //        }
        //    }
        //    return true;
        //}

        private AssignmentExpression FindAssignment(Identifier id)
        {
            var block = id.Ancestors.OfType<BlockStatement>().First();

            AssignmentExpression assignment = null;

            foreach (var s in block.Descendants.OfType<IdentifierExpression>()) {
                if (s.Identifier == id.Name) {
                    var parent = s.Parent;
                    if (parent is MemberReferenceExpression || parent is BinaryOperatorExpression || parent is UnaryOperatorExpression || parent is CastExpression || parent is AsExpression || parent is ObjectCreateExpression) continue;
                    if (parent is AssignmentExpression && assignment == null) assignment = parent as AssignmentExpression;
                    else return null; // TODO: more checks
                }
            }
            return assignment;
        }

        //private bool IsReadonlyVariable(Identifier id)
        //{
        //    var block = id.Ancestors.OfType<BlockStatement>().First();

        //    foreach (var s in block.Descendants.OfType<IdentifierExpression>()) {
        //        if (s.Identifier == id.Name) {
        //            var parent = s.Parent;
        //            if (parent is MemberReferenceExpression || parent is BinaryOperatorExpression || parent is UnaryOperatorExpression || parent is CastExpression || parent is AsExpression) continue;
        //            return false; // TODO: more checks
        //        }
        //    }
        //    return true;
        //}
    }
}
