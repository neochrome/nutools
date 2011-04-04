using System;
using System.IO;
using System.Linq;
using System.Text;

using Cone;

using NuTools.Common;

namespace NuTools.Specs
{
    [Describe(typeof(OptionParser))]
    public class OptionParserSpecs
    {
        public void should_parse_when_empty_and_no_args()
        {
            var opts = new OptionParser();
            Verify.That(() => opts.Parse(new string[] { }) == true);
        }

        public void should_parse_multiple_options()
        {
            var retries = 0;
            var verbose = false;
            var file = "";
            var opts = new OptionParser();
            opts.On("retries", "number of {0} to retry the operation").WithArg<int>("TIMES").Do(value => retries = value);
            opts.On("verbose", "Verbose output").Do(() => verbose = true);
            opts.Arg<string>("FILE", "The input").Do(value => file = value);

            opts.Parse(new[] { "--verbose", "source.txt", "--retries=10" });

            Verify.That(() => retries == 10);
            Verify.That(() => verbose == true);
            Verify.That(() => file == "source.txt");
        }

        public void should_support_multiple_groups()
        {
            bool a, b, c; a = b = c = false;
            var opts = new OptionParser();
            opts.On('a', "first").Do(() => a = true);
            opts.In("group1", g => g.On('b', "second").Do(() => b = true));
            opts.In("group2", g => g.On('c', "third").Do(() => c = true));

            opts.Parse(new[] { "-abc" });
            Verify.That(() => (a && b && c) == true);
        }

        [Context("summary")]
        public class Summary
        {
            public void should_have_banner_first()
            {
                var opts = new OptionParser();
                opts.Banner = "the banner";
                Verify.That(() => opts.Summary().StartsWith(opts.Banner));
            }

            [Pending]
            public void should_have_aligned_descriptions()
            {
                var opts = new OptionParser();
                opts.Banner = "the banner";
                opts.On("long", 'a', "description").Do(() => { });
                opts.On("longer", 'b', "description").Do(() => { });
                opts.On("longest", 'c', "description").Do(() => { });
                var summary = opts.Summary();
                var reader = new StringReader(summary);
            }
        }
        
        [Pending]
        public void should_display_summary()
        {
            var summary = string.Empty;

            var opts = new OptionParser();
            opts.Banner = "Usage: program [options]";
            opts.On("retries", "number of {0} to retry the operation").WithArg<int>("TIMES");
            opts.On("verbose", "Verbose output");
            opts.Required.Arg<string>("FILE", "The input");
            opts.On("help", "Displays this message").Do(() => summary = opts.Summary());

            opts.Parse(new[] { "--help" });

            Verify.That(() => summary.Contains(opts.Banner));
            Verify.That(() => summary.Contains("[NUM]"));
            Verify.That(() => summary.Contains("--retries"));
            Verify.That(() => summary.Contains("NUM retries"));
            Verify.That(() => summary.Contains("--verbose"));
            Verify.That(() => summary.Contains("-v"));
            Verify.That(() => summary.Contains("Verbose output"));
            Verify.That(() => summary.Contains("FILE"));
            Verify.That(() => summary.Contains("The input"));
            Verify.That(() => summary.Contains("--help"));
            Verify.That(() => summary.Contains("Displays this message"));
        }

        [Context("option")]
        public class Option
        {
            public void should_be_executed_when_present()
            {
                var executed = false;
                var opts = new OptionParser();
                opts.On("switch", "A switch").Do(() => executed = true);

                opts.Parse(new[] { "--switch" });

                Verify.That(() => executed == true);
            }

            public void should_parse_group()
            {
                bool a = false, b = false, c = false, d = false;
                var opts = new OptionParser();
                opts.On("aaa", 'a', "desc").Do(() => a = true);
                opts.On("bbb", 'b', "desc").Do(() => b = true);
                opts.On("ccc", 'c', "desc").Do(() => c = true);
                opts.On("ddd", 'd', "desc").Do(() => d = true);

                var result = opts.Parse(new[] { "-abd" });

                Verify.That(() => result == true);
                Verify.That(() => a == true);
                Verify.That(() => b == true);
                Verify.That(() => c == false);
                Verify.That(() => d == true);
            }
        }

        [Context("option with argument")]
        public class OptionWithArgument
        {
            public void should_handle_name_with_dashes()
            {
                var value = "";
                var opts = new OptionParser();
                opts.On("long-switch", "A switch").WithArg<string>("VALUE").Do(t => value = t);

                opts.Parse(new[] { "--long-switch=value" });

                Verify.That(() => value == "value");
            }

            public void should_fail_when_missing_argument()
            {
                var opts = new OptionParser();
                opts.On("retries", "number of {0} to retry the operation").WithArg<int>("TIMES");
                Verify.That(() => opts.Parse(new string[] { "--retries" }) == false);
            }

            [Context("should receive value of")]
            public class Values
            {
                public void boolean()
                {
                    bool? value = null;
                    var option = new OptionWithArgument<bool>();
                    option.Do(v => value = v);

                    option.Receive(true.ToString());

                    Verify.That(() => value == true);
                }

                public void integer()
                {
                    int? value = null;
                    var option = new OptionWithArgument<int>();
                    option.Do(v => value = v);

                    option.Receive(42.ToString());

                    Verify.That(() => value == 42);
                }

                public void @string()
                {
                    string value = null;
                    var option = new OptionWithArgument<string>();
                    option.Do(v => value = v);

                    option.Receive("hello");

                    Verify.That(() => value == "hello");
                }

                public void date()
                {
                    var expected = DateTime.Today;
                    DateTime? value = null;
                    var option = new OptionWithArgument<DateTime>();
                    option.Do(v => value = v);

                    option.Receive(expected.ToString());

                    Verify.That(() => value == expected);
                }
            }
        }

        [Context("argument")]
        public class Argument
        {
            public void should_receive_value()
            {
                var value = "";
                var opts = new OptionParser();
                opts.Arg<string>("FILE", "The input filename)").Do(t => value = t);

                opts.Parse(new[] { "somefile.txt" });

                Verify.That(() => value == "somefile.txt");
            }

            public void should_receive_dash_value()
            {
                var value = "";
                var opts = new OptionParser();
                opts.Arg<string>("FILE", "The input (use - for stdin)").Do(t => value = t);

                opts.Parse(new[] { "-" });

                Verify.That(() => value == "-");
            }

            public void should_fail_if_missing_when_required()
            {
                var opts = new OptionParser();
                opts.Required.Arg<int>("RETRIES", "number of {0} to retry the operation");
                Verify.That(() => opts.Parse(new string[] { }) == false);
            }
        }

    }
}