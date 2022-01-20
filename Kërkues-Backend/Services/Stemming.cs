/*
 * The Porter2 stemming algorithm
 *
 * author: Kamil Bartocha
 * email: crow2 (at) poczta.fm
 * version: 1.0.0
 * date: 11.04.2007
 *
 *
 * This is a C# implementation of the Porter2 algorithm as described on:
 * http://snowball.tartarus.org/algorithms/english/stemmer.html
 *
 * It is provided as a ready-to-use class with only a single public method:
 *
 * public string stem(string word)
 *
 *
 * Usage example:
 *
 * stemer = new Porter2();
 * string output = stemer.stem("word");
 *
 *
 * If you notice any flaws or ways to improve this source code, or simply decide
 * to use it in your project, please let me know.
 *
 * Notices for users:
 *
 * 1. The sb.Replace(...) code might be somehow optimized but this
 * seemed to be the best choice to do it.
 *
 * 2. I've tried to use the String class more often insted of the sb.ToString(...)
 * but the resulting code was 100% SLOWER! I guess it's because of StringBuilder's
 * internal optimizations.
 *
 *
 * LICENSE:
 *
 * Copyright 2007 Kamil Bartocha. All rights reserved.
 *
 * Redistribution and use in source and binary forms,
 * with or without modification, are permitted provided that
 * the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY KAMIL BARTOCHA ``AS IS''
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE FREEBSD PROJECT OR CONTRIBUTORS
 * BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
 * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT
 * OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 *
 */

using System;
using System.Text;

namespace Kërkues_Backend.Services
{
    public class Porter2
    {
        private readonly string[] _doubles = { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        private readonly string[] _validLiEndings = { "c", "d", "e", "g", "h", "k", "m", "n", "r", "t" };

        private readonly string[,] _step1BReplacements =
        {
            { "eedly", "ee" },
            { "ingly", "" },
            { "edly", "" },
            { "eed", "ee" },
            { "ing", "" },
            { "ed", "" }
        };

        private readonly string[,] _step2Replacements =
        {
            { "ization", "ize" },
            { "iveness", "ive" },
            { "fulness", "ful" },
            { "ational", "ate" },
            { "ousness", "ous" },
            { "biliti", "ble" },
            { "tional", "tion" },
            { "lessli", "less" },
            { "fulli", "ful" },
            { "entli", "ent" },
            { "ation", "ate" },
            { "aliti", "al" },
            { "iviti", "ive" },
            { "ousli", "ous" },
            { "alism", "al" },
            { "abli", "able" },
            { "anci", "ance" },
            { "alli", "al" },
            { "izer", "ize" },
            { "enci", "ence" },
            { "ator", "ate" },
            { "bli", "ble" },
            { "ogi", "og" },
            { "li", "" }
        };

        private readonly string[,] _step3Replacements =
        {
            { "ational", "ate" },
            { "tional", "tion" },
            { "alize", "al" },
            { "icate", "ic" },
            { "iciti", "ic" },
            { "ative", "" },
            { "ical", "ic" },
            { "ness", "" },
            { "ful", "" }
        };

        private readonly string[] _step4Replacements =
        {
            "ement",
            "ment",
            "able",
            "ible",
            "ance",
            "ence",
            "ate",
            "iti",
            "ion",
            "ize",
            "ive",
            "ous",
            "ant",
            "ism",
            "ent",
            "al",
            "er",
            "ic"
        };

        private readonly string[,] _exceptions =
        {
            { "skis", "ski" },
            { "skies", "sky" },
            { "dying", "die" },
            { "lying", "lie" },
            { "tying", "tie" },
            { "idly", "idl" },
            { "gently", "gentl" },
            { "ugly", "ugli" },
            { "early", "earli" },
            { "only", "onli" },
            { "singly", "singl" },
            { "sky", "sky" },
            { "news", "news" },
            { "howe", "howe" },
            { "atlas", "atlas" },
            { "cosmos", "cosmos" },
            { "bias", "bias" },
            { "andes", "andes" }
        };

        private readonly string[] _exceptions2 =
        {
            "inning", "outing", "canning", "herring", "earring", "proceed",
            "exceed", "succeed"
        };


        // A helper table lookup code - used for vowel lookup
        private bool arrayContains(string[] arr, string s)
        {
            return arr.Any(t => t == s);
        }

        private bool isVowel(StringBuilder s, int offset)
        {
            switch (s[offset])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                case 'y':
                    return true;
                default:
                    return false;
            }
        }

        private bool isShortSyllable(StringBuilder s, int offset)
        {
            return offset == 0 && isVowel(s, 0) && !isVowel(s, 1) ||
                   offset > 0 && offset < s.Length - 1 &&
                   isVowel(s, offset) && !isVowel(s, offset + 1) &&
                   s[offset + 1] != 'w' && s[offset + 1] != 'x' && s[offset + 1] != 'Y'
                   && !isVowel(s, offset - 1);
        }

        private bool isShortWord(StringBuilder s, int r1)
        {
            return r1 >= s.Length && isShortSyllable(s, s.Length - 2);
        }

        private void changeY(StringBuilder sb)
        {
            if (sb[0] == 'y') sb[0] = 'Y';

            for (var i = 1; i < sb.Length; ++i)
            {
                if (sb[i] == 'y' && isVowel(sb, i - 1)) 
                    sb[i] = 'Y';
            }
        }

        private void computeR1R2(StringBuilder sb, ref int r1, ref int r2)
        {
            r1 = sb.Length;
            r2 = sb.Length;

            if (sb.Length >= 5 && (sb.ToString(0, 5) == "gener" || sb.ToString(0, 5) == "arsen")) r1 = 5;
            if (sb.Length >= 6 && (sb.ToString(0, 6) == "commun")) r1 = 6;

            if (r1 == sb.Length) // If R1 has not been changed by exception words
                for (var i = 1; i < sb.Length; ++i) // Compute R1 according to the algorithm
                {
                    if (!isVowel(sb, i) && isVowel(sb, i - 1))
                    {
                        r1 = i + 1;
                        break;
                    }
                }

            for (var i = r1 + 1; i < sb.Length; ++i)
            {
                if (!isVowel(sb, i) && isVowel(sb, i - 1))
                {
                    r2 = i + 1;
                    break;
                }
            }
        }

        private void step0(StringBuilder sb)
        {

            if (sb.Length >= 3 && sb.ToString(sb.Length - 3, 3) == "'s'")
                sb.Remove(sb.Length - 3, 3);
            else if (sb.Length >= 2 && sb.ToString(sb.Length - 2, 2) == "'s")
                sb.Remove(sb.Length - 2, 2);
            else if (sb[sb.Length - 1] == '\'')
                sb.Remove(sb.Length - 1, 1);
        }

        private void step1a(StringBuilder sb)
        {
            if (sb.Length >= 4 && sb.ToString(sb.Length - 4, 4) == "sses")
                sb.Replace("sses", "ss", sb.Length - 4, 4);
            else if (sb.Length >= 3 && (sb.ToString(sb.Length - 3, 3) == "ied" || sb.ToString(sb.Length - 3, 3) == "ies"))
            {
                sb.Replace(sb.ToString(sb.Length - 3, 3), sb.Length > 4 ? "i" : "ie", sb.Length - 3, 3);
            }
            else
            {
                if (sb.Length >= 2 && (sb.ToString(sb.Length - 2, 2) == "us" || sb.ToString(sb.Length - 2, 2) == "ss"))
                    return;
                if (sb.Length > 0 && sb.ToString(sb.Length - 1, 1) == "s")
                {
                    for (var i = 0; i < sb.Length - 2; ++i)
                        if (isVowel(sb, i))
                        {
                            sb.Remove(sb.Length - 1, 1);
                            break;
                        }
                }
            }
        }

        private void step1b(StringBuilder sb, int r1)
        {
            for (var i = 0; i < 6; ++i)
            {
                if (sb.Length > _step1BReplacements[i, 0].Length && sb.ToString(sb.Length - _step1BReplacements[i, 0].Length, _step1BReplacements[i, 0].Length) == _step1BReplacements[i, 0])
                {
                    switch (_step1BReplacements[i, 0])
                    {
                        case "eedly":
                        case "eed":
                            if (sb.Length - _step1BReplacements[i, 0].Length >= r1)
                                sb.Replace(_step1BReplacements[i, 0], _step1BReplacements[i, 1], sb.Length - _step1BReplacements[i, 0].Length, _step1BReplacements[i, 0].Length);
                            break;
                        default:
                            var found = false;
                            for (var j = 0; j < sb.Length - _step1BReplacements[i, 0].Length; ++j)
                            {
                                if (isVowel(sb, j))
                                {
                                    sb.Replace(_step1BReplacements[i, 0], _step1BReplacements[i, 1], sb.Length - _step1BReplacements[i, 0].Length, _step1BReplacements[i, 0].Length);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) return;
                            if (sb.Length >= 2)
                            {
                                switch (sb.ToString(sb.Length - 2, 2))
                                {
                                    case "at":
                                    case "bl":
                                    case "iz":
                                        sb.Append("e");
                                        return;
                                }
                                if (arrayContains(_doubles, sb.ToString(sb.Length - 2, 2)))
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                    return;
                                }
                            }
                            if (isShortWord(sb, r1))
                                sb.Append("e");
                            break;
                    }
                    return;
                }
            }
        }

        private void step1c(StringBuilder sb)
        {
            if (sb.Length > 0 &&
                (sb[sb.Length - 1] == 'y' || sb[sb.Length - 1] == 'Y') &&
                sb.Length > 2 && (!isVowel(sb, sb.Length - 2))
               )
                sb[sb.Length - 1] = 'i';
        }

        private void step2(StringBuilder sb, int r1)
        {
            for (var i = 0; i < 24; ++i)
            {
                if (
                    sb.Length >= _step2Replacements[i, 0].Length &&
                    sb.ToString(sb.Length - _step2Replacements[i, 0].Length, _step2Replacements[i, 0].Length) == _step2Replacements[i, 0]
                    )
                {
                    if (sb.Length - _step2Replacements[i, 0].Length >= r1)
                    {
                        switch (_step2Replacements[i, 0])
                        {
                            case "ogi":
                                if ((sb.Length > 3) &&
                                    (sb[sb.Length - _step2Replacements[i, 0].Length - 1] == 'l')
                                    )
                                    sb.Replace(_step2Replacements[i, 0], _step2Replacements[i, 1], sb.Length - _step2Replacements[i, 0].Length, _step2Replacements[i, 0].Length);
                                return;
                            case "li":
                                if ((sb.Length > 1) &&
                                    (arrayContains(_validLiEndings, sb.ToString(sb.Length - 3, 1)))
                                    )
                                    sb.Remove(sb.Length - 2, 2);
                                return;
                            default:
                                sb.Replace(_step2Replacements[i, 0], _step2Replacements[i, 1], sb.Length - _step2Replacements[i, 0].Length, _step2Replacements[i, 0].Length);
                                return;
                        }
                    }
                    else
                        return;
                }
            }
        }

        private void step3(StringBuilder sb, int r1, int r2)
        {
            for (var i = 0; i < 9; ++i)
            {
                if (
                    sb.Length >= _step3Replacements[i, 0].Length &&
                    sb.ToString(sb.Length - _step3Replacements[i, 0].Length, _step3Replacements[i, 0].Length) == _step3Replacements[i, 0]
                    )
                {
                    if (sb.Length - _step3Replacements[i, 0].Length >= r1)
                    {
                        switch (_step3Replacements[i, 0])
                        {
                            case "ative":
                                if (sb.Length - _step3Replacements[i, 0].Length >= r2)
                                    sb.Replace(_step3Replacements[i, 0], _step3Replacements[i, 1], sb.Length - _step3Replacements[i, 0].Length, _step3Replacements[i, 0].Length);
                                return;
                            default:
                                sb.Replace(_step3Replacements[i, 0], _step3Replacements[i, 1], sb.Length - _step3Replacements[i, 0].Length, _step3Replacements[i, 0].Length);
                                return;
                        }
                    }
                    else return;
                }
            }
        }

        private void step4(StringBuilder sb, int r2)
        {
            for (var i = 0; i < 18; ++i)
            {
                if (
                    sb.Length >= _step4Replacements[i].Length &&
                    sb.ToString(sb.Length - _step4Replacements[i].Length, _step4Replacements[i].Length) == _step4Replacements[i]                    // >=
                    )
                {
                    if (sb.Length - _step4Replacements[i].Length >= r2)
                    {
                        switch (_step4Replacements[i])
                        {
                            case "ion":
                                if (
                                    sb.Length > 3 &&
                                    (
                                        sb[sb.Length - _step4Replacements[i].Length - 1] == 's' ||
                                        sb[sb.Length - _step4Replacements[i].Length - 1] == 't'
                                    )
                                   )
                                    sb.Remove(sb.Length - _step4Replacements[i].Length, _step4Replacements[i].Length);
                                return;
                            default:
                                sb.Remove(sb.Length - _step4Replacements[i].Length, _step4Replacements[i].Length);
                                return;
                        }
                    }

                    return;
                }
            }

        }

        private void step5(StringBuilder sb, int r1, int r2)
        {
            if (sb.Length > 0 &&
                (sb[sb.Length - 1] == 'e' &&
                 (sb.Length - 1 >= r2 || sb.Length - 1 >= r1 && !isShortSyllable(sb, sb.Length - 3)) ||
                 sb[sb.Length - 1] == 'l' && sb.Length - 1 >= r2 && sb[sb.Length - 2] == 'l'))
                sb.Remove(sb.Length - 1, 1);
        }

        public string stem(string word)
        {
            if (word.Length < 3) return word;

            var sb = new StringBuilder(word.ToLower());

            if (sb[0] == '\'') sb.Remove(0, 1);

            for (var i = 0; i < _exceptions.Length / 2; ++i)
                if (word == _exceptions[i, 0])
                    return _exceptions[i, 1];

            int r1 = 0, r2 = 0;
            changeY(sb);
            computeR1R2(sb, ref r1, ref r2);

            step0(sb);
            step1a(sb);

            foreach (var e in _exceptions2)
                if (sb.ToString() == e)
                    return e;

            step1b(sb, r1);
            step1c(sb);
            step2(sb, r1);
            step3(sb, r1, r2);
            step4(sb, r2);
            step5(sb, r1, r2);


            return sb.ToString().ToLower();
        }
    }
}