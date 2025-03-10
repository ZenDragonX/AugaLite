﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/*
http://forum.unity3d.com/threads/adjustable-character-spacing-free-script.288277/
Unity 5.4 compatible version
Produces an simple tracking/letter-spacing effect on UI Text components.
Set the spacing parameter to adjust letter spacing.

Negative values cuddle the text up tighter than normal. Go too far and it'll look odd.
Positive values spread the text out more than normal. This will NOT respect the text area you've defined.
Zero spacing will present the font with no changes.
Relies on counting off characters in your Text component's text property and
matching those against the quads passed in via the verts array. This is really
rather primative, but I can't see any better way at the moment. It means that
all sorts of things can break the effect...

This component should be placed higher in component list than any other vertex
modifiers that alter the total number of verticies. Eg, place this ABOVE Shadow
or Outline effects. If you don't, the outline/shadow won't match the position
of the letters properly. If you place the outline/shadow effect second however,
it will just work on the altered vertices from this component, and function
as expected.

This component works best if you don't allow text to automatically wrap. It also
blows up outside of the given text area. Basically, it's a cheap and dirty effect,
not a clever text layout engine. It can't affect how Unity chooses to break up
your lines. If you manually use line breaks however, it should detect those and
function more or less as you'd expect.

The spacing parameter is measured in pixels multiplied by the font size. This was
chosen such that when you adjust the font size, it does not change the visual spacing
that you've dialed in. There's also a scale factor of 1/100 in this number to
bring it into a comfortable adjustable range. There's no limit on this parameter,
but obviously some values will look quite strange.

Now component works with RichText. You need to remember to turn on RichText via the checkbox (text.supportRichText)
and turn on component's [useRichText] checkbox.
*/

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("UI/Effects/Letter Spacing", 15)]
    public class LetterSpacing : BaseMeshEffect
    {
        private const string SupportedTagRegexPattersn = @"<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

        public bool UseRichText;
        public float Spacing;

        protected LetterSpacing()
        {
        }

        private float _lastSpacing;
        public void Update()
        {
            if (Application.isEditor)
            {
                if (Spacing != _lastSpacing)
                {
                    _lastSpacing = Spacing;
                    if (graphic != null)
                    {
                        graphic.SetVerticesDirty();
                    }
                }
            }
        }

        /**
        * Note: Unity 5.2.1 ModifyMesh(Mesh mesh) used VertexHelper.FillMesh(mesh);
        * For performance reasons, ModifyMesh(VertexHelper vh) was introduced
        * @see http://forum.unity3d.com/threads/unity-5-2-ui-performance-seems-much-worse.353650/
        */
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            var list = new List<UIVertex>();
            vh.GetUIVertexStream(list);

            ModifyVertices(list);

            vh.Clear();
            vh.AddUIVertexTriangleStream(list);
        }

        public void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
            {
                return;
            }

            var text = GetComponent<Text>();

            if (text == null)
            {
                Debug.LogWarning("LetterSpacing: Missing Text component");
                return;
            }

            var str = text.text;

            // Artificially insert line breaks for automatic line breaks.
            var lineInfos = text.cachedTextGenerator.lines;
            for (var i = lineInfos.Count - 1; i > 0; i--)
            {
                // Insert a \n at the location Unity wants to automatically line break.
                // Also, remove any space before the automatic line break location.
                str = str.Insert(lineInfos[i].startCharIdx, "\n");
                str = str.Remove(lineInfos[i].startCharIdx - 1, 1);
            }

            var lines = str.Split('\n');

            var letterOffset = Spacing * text.fontSize / 100f;
            float alignmentFactor = 0;
            var glyphIdx = 0; // character index from the beginning of the text, including RichText tags and line breaks

            var isRichText = UseRichText && text.supportRichText;
            IEnumerator matchedTagCollection = null; // when using RichText this will collect all tags (index, length, value)
            Match currentMatchedTag = null;

            switch (text.alignment)
            {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    alignmentFactor = 0f;
                    break;

                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    alignmentFactor = 0.5f;
                    break;

                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    alignmentFactor = 1f;
                    break;
            }

            for (var lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                var line = lines[lineIdx];
                var lineLength = line.Length;

                if (isRichText)
                {
                    matchedTagCollection = GetRegexMatchedTagCollection(line, out lineLength);
                    currentMatchedTag = null;
                    if (matchedTagCollection.MoveNext())
                    {
                        currentMatchedTag = (Match) matchedTagCollection.Current;
                    }
                }

                var lineOffset = (lineLength - 1) * letterOffset * alignmentFactor;

                for (int charIdx = 0, actualCharIndex = 0; charIdx < line.Length; charIdx++, actualCharIndex++)
                {
                    if (isRichText)
                    {
                        if (currentMatchedTag != null && currentMatchedTag.Index == charIdx)
                        {
                            // skip matched RichText tag
                            charIdx += currentMatchedTag.Length - 1; // -1 because next iteration will increment charIdx
                            actualCharIndex--; // tag is not an actual character, cancel counter increment on this iteration
                            glyphIdx += currentMatchedTag.Length; // glyph index is not incremented in for loop so skip entire length

                            // prepare next tag to detect
                            currentMatchedTag = null;
                            if (matchedTagCollection.MoveNext())
                            {
                                currentMatchedTag = (Match) matchedTagCollection.Current;
                            }

                            continue;
                        }
                    }

                    var idx1 = glyphIdx * 6 + 0;
                    var idx2 = glyphIdx * 6 + 1;
                    var idx3 = glyphIdx * 6 + 2;
                    var idx4 = glyphIdx * 6 + 3;
                    var idx5 = glyphIdx * 6 + 4;
                    var idx6 = glyphIdx * 6 + 5;

                    // Check for truncated text (doesn't generate verts for all characters)
                    if (idx6 > verts.Count - 1)
                    {
                        return;
                    }

                    var vert1 = verts[idx1];
                    var vert2 = verts[idx2];
                    var vert3 = verts[idx3];
                    var vert4 = verts[idx4];
                    var vert5 = verts[idx5];
                    var vert6 = verts[idx6];

                    var pos = Vector3.right * (letterOffset * actualCharIndex - lineOffset);

                    vert1.position += pos;
                    vert2.position += pos;
                    vert3.position += pos;
                    vert4.position += pos;
                    vert5.position += pos;
                    vert6.position += pos;

                    verts[idx1] = vert1;
                    verts[idx2] = vert2;
                    verts[idx3] = vert3;
                    verts[idx4] = vert4;
                    verts[idx5] = vert5;
                    verts[idx6] = vert6;

                    glyphIdx++;
                }

                // Offset for carriage return character that still generates verts
                glyphIdx++;
            }
        }

        private static IEnumerator GetRegexMatchedTagCollection(string line, out int lineLengthWithoutTags)
        {
            var matchedTagCollection = Regex.Matches(line, SupportedTagRegexPattersn);
            lineLengthWithoutTags = 0;
            var tagsLength = 0;

            if (matchedTagCollection.Count > 0)
            {
                foreach (Match matchedTag in matchedTagCollection)
                {
                    tagsLength += matchedTag.Length;
                }
            }

            lineLengthWithoutTags = line.Length - tagsLength;
            return matchedTagCollection.GetEnumerator();
        }
    }
}