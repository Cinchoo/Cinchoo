using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Cinchoo.Core
{
    public enum ChoTokenDefEnum
    {
        [Description(@"([""'])(?:\\\1|.)*?\1")]
        QUOTED_STRING,
        [Description(@"[-+]?\d*\.\d+([eE][-+]?\d+)?")]
        FLOAT,
        [Description(@"[-+]?\d+")]
        INT,
        [Description(@"#t")]
        TRUE,
        [Description(@"#f")]
        FALSE,
        [Description(@"[*<>\?\-+/A-Za-z->!_0-9]+")]
        SYMBOL,
        [Description(@"\.")]
        DOT,
        [Description(@"\(")]
        LEFT,
        [Description(@"\)")]
        RIGHT,
        [Description(@"\s")]
        SPACE
    }

    public enum ChoTokenDefType { Value, UnaryOp, BinaryOp }

    public sealed class ChoTokenContent<T>
    {
        public readonly T Token;
        public readonly ChoTokenDefType TokenDefType;
        public readonly string TokenContent;

        public ChoTokenContent(T token, string tokenContent, ChoTokenDefType tokenDefType = ChoTokenDefType.Value)
        {
            Token = token;
            TokenDefType = tokenDefType;
            TokenContent = tokenContent;
        }
    }

    public sealed class ChoTokenDefinition<T>
    {
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> QuotedStringTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.QUOTED_STRING), ChoTokenDefEnum.QUOTED_STRING);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> FloatTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.FLOAT), ChoTokenDefEnum.FLOAT);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> IntTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.INT), ChoTokenDefEnum.INT);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> TrueTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.TRUE), ChoTokenDefEnum.TRUE);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> FalseTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.FALSE), ChoTokenDefEnum.FALSE);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> SymbolTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.SYMBOL), ChoTokenDefEnum.SYMBOL);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> DotTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.DOT), ChoTokenDefEnum.DOT);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> LeftTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.LEFT), ChoTokenDefEnum.LEFT);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> RightTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.RIGHT), ChoTokenDefEnum.RIGHT);
        public static readonly ChoTokenDefinition<ChoTokenDefEnum> SpaceTokenDef = new ChoTokenDefinition<ChoTokenDefEnum>(ChoEnum.ToDescription(ChoTokenDefEnum.SPACE), ChoTokenDefEnum.SPACE);

        public static readonly ChoTokenDefinition<ChoTokenDefEnum>[] AllTokenDef =
            new ChoTokenDefinition<ChoTokenDefEnum>[] 
            {
                QuotedStringTokenDef,
                FloatTokenDef,
                IntTokenDef,
                TrueTokenDef,
                FalseTokenDef,
                SymbolTokenDef,
                DotTokenDef,
                LeftTokenDef,
                RightTokenDef,
                SpaceTokenDef
            };

        public readonly IChoMatcher Matcher;
        public readonly T Token;
        public readonly ChoTokenDefType TokenDefType;

        public ChoTokenDefinition(string regex, T token, ChoTokenDefType tokenDefType = ChoTokenDefType.Value)
        {
            this.Matcher = new ChoRegexMatcher(regex);
            this.Token = token;
            TokenDefType = tokenDefType;
        }
    }
}
