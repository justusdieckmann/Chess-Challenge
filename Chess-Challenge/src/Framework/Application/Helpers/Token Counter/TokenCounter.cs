using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace ChessChallenge.Application
{
    public static class TokenCounter
    {

        static readonly HashSet<SyntaxKind> tokensToIgnore = new(new SyntaxKind[]
        {
            SyntaxKind.PrivateKeyword,
            SyntaxKind.PublicKeyword,
            SyntaxKind.SemicolonToken,
            SyntaxKind.CommaToken,
            // only count open brace since I want to count the pair as a single token
            SyntaxKind.CloseBraceToken, 
            SyntaxKind.CloseBracketToken,
            SyntaxKind.CloseParenToken
        });

        public static int CountTokens(string code)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            SyntaxNode root = tree.GetRoot();
            return CountTokens(root);
        }

        static int CountTokens(SyntaxNodeOrToken syntaxNode)
        {
            SyntaxKind kind = syntaxNode.Kind();
            int numTokensInChildren = 0;


            foreach (var child in syntaxNode.ChildNodesAndTokens())
            {
                numTokensInChildren += CountTokens(child);
            }

            if (syntaxNode.IsToken && !tokensToIgnore.Contains(kind))
            {
                // System.Console.WriteLine(kind + "  " + syntaxNode.ToString() + " " + syntaxNode.AsToken().ValueText);

                // String literals count for as many chars as are in the string plus one (so that for example an empty string counts as one token)
                if (kind is SyntaxKind.StringLiteralToken or SyntaxKind.InterpolatedStringTextToken or SyntaxKind.MultiLineRawStringLiteralToken)
                {
                    return syntaxNode.AsToken().ValueText.Length + 1;
                }

                // Regular tokens count as just one token
                return 1;
            }

            return numTokensInChildren;
        }

    }
}