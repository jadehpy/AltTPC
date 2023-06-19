using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltTPC {
    internal class Commands {


        //public class Argument { }

        //public class Arg_Primitive : Argument {
        //    public List<NodeType> types;

        //    public Arg_Primitive(List<NodeType> types) {
        //        this.types = types;
        //    }
        //}

        public struct Argument {
            public string[]? word;
            public NodeType[][]? types;

            public Argument(string[]? word, NodeType[][]? types) {
                this.word = word;
                this.types = types;
            }
        }

        //public class Arg_Peculiar : Argument {
        //    public string[] word;
        //    public List<NodeType[]> types;

        //    public Arg_Peculiar(string[] word, List<NodeType[]> types) {
        //        this.word = word;
        //        this.types = types;
        //    }
        //}

        //public struct Arg_Peculiar {
        //    public string[] word;
        //    public List<NodeType[]> types;

        //    public Arg_Peculiar(string[] word, List<NodeType[]> types) {
        //        this.word = word;
        //        this.types = types;
        //    }
        //}

        const bool withID = true;
        const bool noID = false;

        public struct SubCommand {
            
            public string[]? word;
            public bool ID;
            public Argument[] arguments;
            
            public SubCommand(string[] word, bool id, Argument[] arguments) {
                this.word = word;
                ID = id;
                this.arguments = arguments;
            }
        }

        public static bool ContainsSubCommand(string key) {
            if (DB.ContainsKey(Interpreter.MainCommand)) {
                int len = DB[Interpreter.MainCommand].Length;
                for (int i = 0; i < len; i++) {
                    var a = DB[Interpreter.MainCommand][i];
                    if (a.word != null && a.word.Contains(key)) {
                        Interpreter.SubCommandIndex = i;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ContainsArgument(string key) {
            var a = DB[Interpreter.MainCommand][Interpreter.SubCommandIndex].arguments;
            for (int i = 0; i < a.Length; i++) {
                var c = a[i];
                if (c.word != null && c.word.Contains(key)) {
                    Interpreter.ArgumentIndex = i;
                    return true;
                }
                //if (a[i] is Arg_Peculiar o) {
                //    if (o.word.Contains(key)) {
                //        Interpreter.ArgumentIndex = i;
                //        return true;
                //    }
                //}
            }
            return false;
        }

      

        public static Argument[] GetArgumentList() {
            return DB[Interpreter.MainCommand][Interpreter.SubCommandIndex].arguments;
        }

        //public static Argument[] GetArgumentList(string mainCommand, string subCommand) {

        //    //return DB[mainCommand][subCommand].arguments;
        //}

        public static bool HasArgument(string word) {
            //var a = GetArgumentList();
            //for (int i = 0; i < a.Count; i++) {
            //    if (a[i] is Arg_Peculiar p) {
            //        if (p.types.Count > 0) {
            //            return true;
            //        }
            //    }
            //}


            //var a = DB[Interpreter.MainCommand][Interpreter.SubCommandIndex].arguments[Interpreter.ArgumentIndex];
            //if (a is Arg_Peculiar o) {
            //    return o.types.Capacity > 0;
            //} 
            //return false;
            var a = DB[Interpreter.MainCommand][Interpreter.SubCommandIndex].arguments[Interpreter.ArgumentIndex].types;
            if (a != null) {
                return a.Length > 0;
            }
            return false;            
        }



        // コマンドのデータベース。全て小文字で書くこと

        public static Dictionary<string, SubCommand[]> DB = new() {
            {
                "msg", new[] {
                    new SubCommand(new[] { "show" }, noID, new[] {
                        new Argument(null, new[] {
                            new[] {
                                NodeType.STRING,
                                NodeType.NUMBER,
                                NodeType.TKV_V, NodeType.TKV_VV__,
                                NodeType.TKV_S, NodeType.TKV_SV__,
                                NodeType.TKV_T, NodeType.TKV_TV__
                            }
                        })
                    })
                }
            },
            {
                "pic", new[] {
                    new SubCommand(new[] { "show" }, withID, new[] {
                        new Argument(null, new[] {
                            new[] {
                                NodeType.STRING, NodeType.TKV_T, NodeType.TKV_TV
                            }
                        }),
                        new Argument(new[] { "pos" }, new[] {
                            new[] { NodeType.NUMBER, NodeType.NUMBER },
                            new[] { NodeType.TKV_V, NodeType.TKV_V },
                            new[] { NodeType.TKV_VV, NodeType.TKV_VV }
                        }),
                        new Argument(new[] { "center", "topLeft", "bottomLeft", "topRight", "bottomRight", "top", "bottom", "left", "right" }, null),
                        new Argument(new[] { "scrollWithMap" }, null),
                        new Argument(new[] { "useChromakey" }, null),
                        new Argument(new[] { "chromakey" }, new[] {
                            new[] { NodeType.NUMBER }
                        }),
                        new Argument(new[] { "trans", "transparency" }, new[] {
                            new[] { NodeType.NUMBER },
                            new[] { NodeType.TKV_V },
                            new[] { NodeType.TKV_VV }
                        })
                    })
                }
            },
            {
                "ev", new[] {
                    new SubCommand(new[] { "setaction" }, withID, new[] {
                        new Argument(new[] { "player", "boat", "ship", "airship", "self" }, null),
                        new Argument(new[] { "act" }, new[] {
                            new[] { NodeType.BR_MOVEROUTE }
                        }),
                        new Argument(new[] { "freq" }, new[] {
                            new[] { NodeType.TKV_V },
                            new[] { NodeType.TKV_VV }
                        }),
                        new Argument(new[] { "repeat", "skippable", "unskippable" }, null)
                    })
                }
            }
        };
    }    
}
