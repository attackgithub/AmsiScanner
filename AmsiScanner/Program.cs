using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections.ObjectModel;
using NDesk.Options;

namespace AmsiScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = null;
            bool help = false;

            var options = new OptionSet()
            {
                { "i|infile=", "Input file", o => file = o },
                { "h|?|help", "Show this help", o => help = true }
            };

            options.Parse(args);

            if (help || file == null)
            {
                options.WriteOptionDescriptions(Console.Out);
            }
            else
            {
                string buffer = File.ReadAllText(file);

                FindAmsiAstSignatures(buffer);
                FindAmsiPsTokenSignatures(buffer);
            }
        }

        private static NativeFunctions.AMSI_RESULT GetAmsiSignature(IntPtr context, string buffer, IntPtr session)
        {
            NativeFunctions.AMSI_RESULT result;
            NativeFunctions.AmsiScanBuffer(context, buffer, (uint)buffer.Length, "test", session, out result);
            return result;
        }

        private static Ast GetAst(string input)
        {
            return Parser.ParseInput(input, out Token[] tokens, out ParseError[] parseErrors);
        }

        private static Collection<PSToken> GetPsTokens(string input)
        {
            Collection<PSParseError> psParseErrors;
            Collection<PSToken> psTokens = PSParser.Tokenize(input, out psParseErrors);
            return psTokens;
        }

        private static void FindAmsiAstSignatures(string input)
        {
            Console.WriteLine();
            Console.WriteLine(" === AST Signatures ===");

            NativeFunctions.AmsiInitialize("AmsiScanner", out IntPtr amsiContext);
            NativeFunctions.AmsiOpenSession(amsiContext, out IntPtr amsiSession);

            Ast tree = GetAst(input);

            var AstSigs = tree.FindAll(
                ast => !string.IsNullOrEmpty(ast.Extent.Text) &&
                (GetAmsiSignature(amsiContext, ast.Extent.Text, amsiSession)) == NativeFunctions.AMSI_RESULT.AMSI_RESULT_DETECTED,
                true);

            var duplicates = AstSigs.GroupBy(o => o.Extent.StartOffset)
                .SelectMany(x => x.Skip(1));

            foreach (Ast ast in AstSigs)
            {
                if (!duplicates.Contains(ast))
                    Console.WriteLine("  {0} == {1}", ast.Extent.Text, NativeFunctions.AMSI_RESULT.AMSI_RESULT_DETECTED);
            }

            NativeFunctions.AmsiCloseSession(amsiContext, amsiSession);
            NativeFunctions.AmsiUninitialize(amsiContext);

        }

        private static void FindAmsiPsTokenSignatures(string input)
        {
            Console.WriteLine();
            Console.WriteLine(" === PSToken Signatures ===");

            NativeFunctions.AmsiInitialize("AmsiScanner", out IntPtr amsiContext);
            NativeFunctions.AmsiOpenSession(amsiContext, out IntPtr amsiSession);

            Collection<PSToken> tokens = GetPsTokens(input);

            Collection<PSToken> tokenSigs = new Collection<PSToken>();

            foreach (PSToken token in tokens)
            {
                if (!string.IsNullOrEmpty(token.Content) &&
                    (GetAmsiSignature(amsiContext, token.Content, amsiSession)) == NativeFunctions.AMSI_RESULT.AMSI_RESULT_DETECTED)

                    tokenSigs.Add(token);
            }

            foreach (PSToken sig in tokenSigs)
            {
                Console.Write("  {0} == {1}", sig.Content, NativeFunctions.AMSI_RESULT.AMSI_RESULT_DETECTED);
            }

            Console.WriteLine();

            NativeFunctions.AmsiCloseSession(amsiContext, amsiSession);
            NativeFunctions.AmsiUninitialize(amsiContext);

        }

    }
}