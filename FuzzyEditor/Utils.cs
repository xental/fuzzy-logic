using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using fuzzy_logic;

namespace FuzzyEditor
{
    public static class Utils
    {
        public static fuzzy_logic.Controller ReadXML(string path) {
            fuzzy_logic.Controller ctrl = new fuzzy_logic.Controller();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode config = xmlDoc.SelectSingleNode("configuration");

            XmlNode variables = config.SelectSingleNode("variables");
            XmlNode rules = config.SelectSingleNode("rules");

            foreach (XmlNode variable in variables.SelectNodes("variable"))
            {
                double ledge = 0;
                double redge = 0;
                string name = "";

                foreach (XmlNode node in variable.ChildNodes)
                {
                    if (node.Name == "name")
                    {
                        name = node.InnerText;
                    } 
                    else if (node.Name == "ledge")
                    {
                        ledge = double.Parse(node.InnerText);
                    }
                    else if (node.Name == "redge")
                    {
                        redge = double.Parse(node.InnerText);
                    } 
                }

                fuzzy_logic.Variable fvar = new fuzzy_logic.Variable(ledge, redge);
                XmlNode terms = variable.SelectSingleNode("terms");
                foreach (XmlNode term in terms.SelectNodes("term"))
                {
                    List<double> args = new List<double>();
                    string tname = "";
                    fuzzy_logic.mfunc mf = mfunc.trimf;

                    foreach (XmlNode node in term.ChildNodes)
                    {
                        if (node.Name == "name")
                        {
                            tname = node.InnerText;
                        }
                        else if (node.Name == "args")
                        {
                            foreach(string value in node.InnerText.Split(';'))
                            {
                                args.Add(double.Parse(value));
                            }
                        }
                        else if (node.Name == "func")
                        {
                            mf = (mfunc)Enum.Parse(typeof(mfunc), node.InnerText, true);
                        }

                    }

                    fvar.RegisterTerm(tname, mf, args);
                }

                ctrl.AddVariable(name, fvar);
            }

            foreach (XmlNode rule in rules.SelectNodes("rule"))
            {
                fuzzy_logic.roperator oper = fuzzy_logic.roperator.and;
                double importance = 0;
                List<(string, string)> condition = new List<(string, string)>();


                foreach (XmlNode node in rule.ChildNodes)
                {
                    if (node.Name == "oper")
                    {
                        oper = (roperator)Enum.Parse(typeof(roperator), node.InnerText, true);
                    } 
                    else if (node.Name == "importance")
                    {
                        importance = double.Parse(node.InnerText);
                    }
                }

                XmlNode conds = rule.SelectSingleNode("conditions");
                foreach (XmlNode cond in conds.SelectNodes("condition"))
                {
                    string v_cond = "";
                    string t_cond = "";

                    foreach (XmlNode node in cond.ChildNodes)
                    {
                        if (node.Name == "variable")
                        {
                            v_cond = node.InnerText;
                        }
                        else
                        {
                            t_cond = node.InnerText;
                        }
                    }

                    condition.Add((v_cond, t_cond));

                }

                string v_conc = "";
                string t_conc = "";

                foreach (XmlNode concs in rule.SelectSingleNode("conclusion"))
                {
                    if (concs.Name == "variable")
                    {
                        v_conc = concs.InnerText;
                    }
                    else
                    {
                        t_conc = concs.InnerText;
                    }
                }

                (string, string) conclusion = (v_conc, t_conc);

                fuzzy_logic.Rule frule = new fuzzy_logic.Rule(condition, conclusion, oper, importance);

                ctrl.AddRule(frule);


            }

            return ctrl;

        }
        public static void SaveXML(string path, fuzzy_logic.Controller ctrl)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("   ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;
            Console.WriteLine(path);
            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartElement("configuration");
                writer.WriteStartElement("variables");
                foreach(KeyValuePair<string, fuzzy_logic.Variable> entry in ctrl.variables){
                    writer.WriteStartElement("variable");
                    writer.WriteElementString("name", entry.Key);
                    writer.WriteElementString("ledge", entry.Value.ledge.ToString());
                    writer.WriteElementString("redge", entry.Value.redge.ToString());

                    writer.WriteStartElement("terms");
                    foreach(KeyValuePair<string, fuzzy_logic.Term> term in entry.Value.terms){
                        writer.WriteStartElement("term");
                        writer.WriteElementString("name", term.Key);
                        writer.WriteElementString("func", term.Value.func.ToString());
                        writer.WriteElementString("args", string.Join(";", term.Value.function.GetArgs()));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteStartElement("rules");
                for(int i = 0; i < ctrl.rules.Count; i++){
                    writer.WriteStartElement("rule");
                    writer.WriteElementString("id", i.ToString());
                    writer.WriteElementString("oper", ctrl.rules[i].oper.ToString());
                    writer.WriteElementString("importance", ctrl.rules[i].importance.ToString());

                    writer.WriteStartElement("conditions");
                    foreach((string, string) cond in ctrl.rules[i].condition){
                        writer.WriteStartElement("condition");
                        writer.WriteElementString("variable", cond.Item1);
                        writer.WriteElementString("term", cond.Item2);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("conclusion");
                    writer.WriteElementString("variable", ctrl.rules[i].conclusion.Item1);
                    writer.WriteElementString("term", ctrl.rules[i].conclusion.Item2);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }
        }
    }
}
