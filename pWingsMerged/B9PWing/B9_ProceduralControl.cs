﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWings.B9PWing
{
    using Utility;
    using UI;
    class B9_ProceduralControl : B9_ProceduralWing
    {
        protected WingProperty rootOffset;
        public double RootOffset
        {
            get { return rootOffset.value; }
            set
            {
                rootOffset.value = value;
                UpdateSymmetricGeometry();
            }

        }
        public MeshFilter meshFilterCtrlFrame;
        public MeshFilter meshFilterCtrlSurface;
        public List<MeshFilter> meshFiltersCtrlEdge = new List<MeshFilter>();

        public static MeshReference meshReferenceCtrlFrame;
        public static MeshReference meshReferenceCtrlSurface;
        public static List<MeshReference> meshReferencesCtrlEdge = new List<MeshReference>();

        public static int meshTypeCountEdgeCtrl = 3;

        public static string[] sharedFieldGroupBaseArrayCtrl = new string[] { "sharedBaseOffsetRoot" };

        public override bool isCtrlSrf
        {
            get { return true; }
        }

        [KSPField]
        public float ctrlFraction = 1f;

        public const float costDensityControl = 6500f;

        /// <summary>
        /// control surfaces cant carry fuel
        /// </summary>
        public override bool canBeFueled
        {
            get
            {
                return false;
            }
        }

        public override float updateCost()
        {
            return (float)Math.Round(wingMass * (1f + ArSweepScale / 4f) * (costDensity * (1f - ctrlFraction) + costDensityControl * ctrlFraction), 1);
        }

        public override void SetupProperties()
        {
            if (length != null)
                return;
            if (part.symmetryCounterparts.Count == 0 || part.symmetryCounterparts[0].Modules.GetModule<B9_ProceduralControl>().length == null)
            {
                length = new WingProperty("Length", nameof(length), 1, 2, 0.05, 8);
                tipOffset = new WingProperty("Offset (tip)", nameof(tipOffset), 0, 2, -1, 1);
                rootOffset = new WingProperty("Offset (root)", nameof(rootOffset), 0, 2, -1, 1);
                rootWidth = new WingProperty("Width (root)", nameof(rootWidth), 0.5, 2, 0.05, 1);
                tipWidth = new WingProperty("Width (tip)", nameof(tipWidth), 0.5, 2, 0.05, 1);
                rootThickness = new WingProperty("Thickness (root)", nameof(rootThickness), 0.2, 2, 0.01, 1);
                tipThickness = new WingProperty("Thickness (tip)", nameof(tipThickness), 0.2, 2, 0.01, 1);

                leadingEdgeType = new WingProperty("Shape", nameof(leadingEdgeType), 2, 0, 1, 3);
                rootLeadingEdge = new WingProperty("Width (root)", nameof(rootLeadingEdge), 0.24, 2, 0.01, 1.0);
                tipLeadingEdge = new WingProperty("Width (tip)", nameof(tipLeadingEdge), 0.24, 2, 0.01, 1.0);

                trailingEdgeType = new WingProperty("Shape", nameof(trailingEdgeType), 3, 0, 1, 3);
                rootTrailingEdge = new WingProperty("Width (root)", nameof(rootTrailingEdge), 0.48, 2, 0.01, 1.0);
                tipTrailingEdge = new WingProperty("Width (tip)", nameof(tipTrailingEdge), 0.48, 2, 0.01, 1.0);

                surfTopMat = new WingProperty("Material", nameof(surfTopMat), 1, 0, 0, 3);
                surfTopOpacity = new WingProperty("Opacity", nameof(surfTopOpacity), 0, 2, 0, 1);
                surfTopHue = new WingProperty("Hue", nameof(surfTopHue), 0.1, 2, 0, 1);
                surfTopSat = new WingProperty("Saturation", nameof(surfTopSat), 0.75, 2, 0, 1);
                surfTopBright = new WingProperty("Brightness", nameof(surfTopBright), 0.6, 2, 0, 1);

                surfBottomMat = new WingProperty("Material", nameof(surfBottomMat), 3, 0, 0, 3);
                surfBottomOpacity = new WingProperty("Opacity", nameof(surfBottomOpacity), 0, 2, 0, 1);
                surfBottomHue = new WingProperty("Hue", nameof(surfBottomHue), 0.1, 2, 0, 1);
                surfBottomSat = new WingProperty("Saturation", nameof(surfBottomSat), 0.75, 2, 0, 1);
                surfBottomBright = new WingProperty("Brightness", nameof(surfBottomBright), 0.6, 2, 0, 1);

                surfLeadMat = new WingProperty("Material", nameof(surfLeadMat), 3, 0, 0, 3);
                surfLeadOpacity = new WingProperty("Opacity", nameof(surfLeadOpacity), 0, 2, 0, 1);
                surfLeadHue = new WingProperty("Hue", nameof(surfLeadHue), 0.1, 2, 0, 1);
                surfLeadSat = new WingProperty("Saturation", nameof(surfLeadSat), 0.75, 2, 0, 1);
                surfLeadBright = new WingProperty("Brightness", nameof(surfLeadBright), 0.6, 2, 0, 1);

                surfTrailMat = new WingProperty("Material", nameof(surfTrailMat), 3, 0, 0, 3);
                surfTrailOpacity = new WingProperty("Opacity", nameof(surfTrailOpacity), 0, 2, 0, 1);
                surfTrailHue = new WingProperty("Hue", nameof(surfTrailHue), 0.1, 2, 0, 1);
                surfTrailSat = new WingProperty("Saturation", nameof(surfTrailSat), 0.75, 2, 0, 1);
                surfTrailBright = new WingProperty("Brightness", nameof(surfTrailBright), 0.6, 2, 0, 1);
            }
            else
            {
                B9_ProceduralControl wp = part.symmetryCounterparts[0].Modules.GetModule<B9_ProceduralControl>();  // all properties for symmetry will be the same object. Yay for no need to update values :D
                length = wp.length;
                tipOffset = wp.tipOffset;
                rootOffset = wp.rootOffset;
                rootWidth = wp.rootWidth;
                tipWidth = wp.tipWidth;
                rootThickness = wp.rootThickness;
                tipThickness = wp.tipThickness;

                leadingEdgeType = wp.leadingEdgeType;
                rootLeadingEdge = wp.rootLeadingEdge;
                tipLeadingEdge = wp.tipLeadingEdge;

                trailingEdgeType = wp.trailingEdgeType;
                rootTrailingEdge = wp.rootTrailingEdge;
                tipTrailingEdge = wp.tipTrailingEdge;

                surfTopMat = wp.surfTopMat;
                surfTopOpacity = wp.surfTopOpacity;
                surfTopHue = wp.surfTopHue;
                surfTopSat = wp.surfTopSat;
                surfTopBright = wp.surfTopBright;

                surfBottomMat = wp.surfBottomMat;
                surfBottomOpacity = wp.surfBottomOpacity;
                surfBottomHue = wp.surfBottomHue;
                surfBottomSat = wp.surfBottomSat;
                surfBottomBright = wp.surfBottomBright;

                surfLeadMat = wp.surfLeadMat;
                surfLeadOpacity = wp.surfLeadOpacity;
                surfLeadHue = wp.surfLeadHue;
                surfLeadSat = wp.surfLeadSat;
                surfLeadBright = wp.surfLeadBright;

                surfTrailMat = wp.surfTrailMat;
                surfTrailOpacity = wp.surfTrailOpacity;
                surfTrailHue = wp.surfTrailHue;
                surfTrailSat = wp.surfTrailSat;
                surfTrailBright = wp.surfTrailBright;
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            try
            {
                rootOffset.Save(node);
            }
            catch
            { }
        }

        public override void LoadWingProperty(ConfigNode n)
        {
            switch (n.GetValue("ID"))
            {
                case nameof(rootOffset):
                    rootOffset.Load(n);
                    break;
                default:
                    base.LoadWingProperty(n);
                    break;
            }
        }


        #region Geometry
        public override void UpdateGeometry(bool updateAerodynamics)
        {
            float ctrlOffsetRootClamped = (float)Utils.Clamp(RootOffset, rootOffset.min, rootOffset.max);
            float ctrlOffsetTipClamped = (float)Utils.Clamp(TipOffset, Math.Max(rootOffset.min, ctrlOffsetRootClamped - Length), rootOffset.max);

            float ctrlThicknessDeviationRoot = (float)RootThickness / 0.24f;
            float ctrlThicknessDeviationTip = (float)TipThickness / 0.24f;

            float ctrlEdgeWidthDeviationRoot = (float)RootTrailingEdge / 0.24f;
            float ctrlEdgeWidthDeviationTip = (float)TipTrailingEdge / 0.24f;

            if (meshFilterCtrlFrame != null)
            {
                int length = meshReferenceCtrlFrame.vp.Length;
                Vector3[] vp = new Vector3[length];
                Array.Copy(meshReferenceCtrlFrame.vp, vp, length);
                Vector3[] nm = new Vector3[length];
                Array.Copy(meshReferenceCtrlFrame.nm, nm, length);
                Vector2[] uv = new Vector2[length];
                Array.Copy(meshReferenceCtrlFrame.uv, uv, length);
                Color[] cl = new Color[length];
                Vector2[] uv2 = new Vector2[length];

                for (int i = 0; i < vp.Length; ++i)
                {
                    // Thickness correction (X), edge width correction (Y) and span-based offset (Z)
                    if (vp[i].z < 0f) vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationTip, vp[i].y, vp[i].z + 0.5f - (float)Length / 2f);
                    else vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationRoot, vp[i].y, vp[i].z - 0.5f + (float)Length / 2f);

                    // Left/right sides
                    if (nm[i] == new Vector3(0f, 0f, 1f) || nm[i] == new Vector3(0f, 0f, -1f))
                    {
                        // Filtering out trailing edge cross sections
                        if (uv[i].y > 0.185f)
                        {
                            // Filtering out root neighbours
                            if (vp[i].y < -0.01f)
                            {
                                if (vp[i].z < 0f)
                                {
                                    vp[i] = new Vector3(vp[i].x, -(float)TipWidth, vp[i].z);
                                    uv[i] = new Vector2((float)TipWidth, uv[i].y);
                                }
                                else
                                {
                                    vp[i] = new Vector3(vp[i].x, -(float)RootWidth, vp[i].z);
                                    uv[i] = new Vector2((float)RootWidth, uv[i].y);
                                }
                            }
                        }
                    }

                    // Root (only needs UV adjustment)
                    else if (nm[i] == new Vector3(0f, 1f, 0f))
                    {
                        if (vp[i].z < 0f) uv[i] = new Vector2((float)Length, uv[i].y);
                    }

                    // Trailing edge
                    else
                    {
                        // Filtering out root neighbours
                        if (vp[i].y < -0.1f)
                        {
                            if (vp[i].z < 0f) vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)TipWidth, vp[i].z);
                            else vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)RootWidth, vp[i].z);
                        }
                    }

                    // Offset-based distortion
                    if (vp[i].z < 0f)
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetTipClamped);
                        if (nm[i] != new Vector3(0f, 0f, 1f) && nm[i] != new Vector3(0f, 0f, -1f)) uv[i] = new Vector2(uv[i].x - (vp[i].y * ctrlOffsetTipClamped) / 4f, uv[i].y);
                    }
                    else
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetRootClamped);
                        if (nm[i] != new Vector3(0f, 0f, 1f) && nm[i] != new Vector3(0f, 0f, -1f)) uv[i] = new Vector2(uv[i].x - (vp[i].y * ctrlOffsetRootClamped) / 4f, uv[i].y);
                    }

                    // Just blanks
                    cl[i] = new Color(0f, 0f, 0f, 0f);
                    uv2[i] = Vector2.zero;
                }

                meshFilterCtrlFrame.mesh.vertices = vp;
                meshFilterCtrlFrame.mesh.uv = uv;
                meshFilterCtrlFrame.mesh.uv2 = uv2;
                meshFilterCtrlFrame.mesh.colors = cl;
                meshFilterCtrlFrame.mesh.RecalculateBounds();

                MeshCollider meshCollider = meshFilterCtrlFrame.gameObject.GetComponent<MeshCollider>();
                if (meshCollider == null)
                    meshCollider = meshFilterCtrlFrame.gameObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = meshFilterCtrlFrame.mesh;
                meshCollider.convex = true;
            }

            // Next, time for edge types
            // Before modifying geometry, we have to show the correct objects for the current selection
            // As UI only works with floats, we have to cast selections into ints too

            int ctrlEdgeTypeInt = Mathf.RoundToInt(TrailingEdgeType - 1);
            for (int i = 0; i < meshTypeCountEdgeCtrl; ++i)
            {
                if (i != ctrlEdgeTypeInt)
                    meshFiltersCtrlEdge[i].gameObject.SetActive(false);
                else
                    meshFiltersCtrlEdge[i].gameObject.SetActive(true);
            }

            // Now we can modify geometry
            // Copy-pasted frame deformation sequence at the moment, to be pruned later

            if (meshFiltersCtrlEdge[ctrlEdgeTypeInt] != null)
            {
                MeshReference meshReference = meshReferencesCtrlEdge[ctrlEdgeTypeInt];
                int length = meshReference.vp.Length;
                Vector3[] vp = new Vector3[length];
                Array.Copy(meshReference.vp, vp, length);
                Vector3[] nm = new Vector3[length];
                Array.Copy(meshReference.nm, nm, length);
                Vector2[] uv = new Vector2[length];
                Array.Copy(meshReference.uv, uv, length);
                Color[] cl = new Color[length];
                Vector2[] uv2 = new Vector2[length];

                for (int i = 0; i < vp.Length; ++i)
                {
                    // Thickness correction (X), edge width correction (Y) and span-based offset (Z)
                    if (vp[i].z < 0f) vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationTip, ((vp[i].y + 0.5f) * ctrlEdgeWidthDeviationTip) - 0.5f, vp[i].z + 0.5f - (float)Length / 2f);
                    else vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationRoot, ((vp[i].y + 0.5f) * ctrlEdgeWidthDeviationRoot) - 0.5f, vp[i].z - 0.5f + (float)Length / 2f);

                    // Left/right sides
                    if (nm[i] == new Vector3(0f, 0f, 1f) || nm[i] == new Vector3(0f, 0f, -1f))
                    {
                        if (vp[i].z < 0f) vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)TipWidth, vp[i].z);
                        else vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)RootWidth, vp[i].z);
                    }

                    // Trailing edge
                    else
                    {
                        // Filtering out root neighbours
                        if (vp[i].y < -0.1f)
                        {
                            if (vp[i].z < 0f) vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)TipWidth, vp[i].z);
                            else vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)RootWidth, vp[i].z);
                        }
                    }

                    // Offset-based distortion
                    if (vp[i].z < 0f)
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetTipClamped);
                        if (nm[i] != new Vector3(0f, 0f, 1f) && nm[i] != new Vector3(0f, 0f, -1f)) uv[i] = new Vector2(uv[i].x - (vp[i].y * ctrlOffsetTipClamped) / 4f, uv[i].y);
                    }
                    else
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetRootClamped);
                        if (nm[i] != new Vector3(0f, 0f, 1f) && nm[i] != new Vector3(0f, 0f, -1f)) uv[i] = new Vector2(uv[i].x - (vp[i].y * ctrlOffsetRootClamped) / 4f, uv[i].y);
                    }

                    // Trailing edge (UV adjustment, has to be the last as it's based on cumulative vertex positions)
                    if (nm[i] != new Vector3(0f, 1f, 0f) && nm[i] != new Vector3(0f, 0f, 1f) && nm[i] != new Vector3(0f, 0f, -1f) && uv[i].y < 0.3f)
                    {
                        if (vp[i].z < 0f) uv[i] = new Vector2(vp[i].z, uv[i].y);
                        else uv[i] = new Vector2(vp[i].z, uv[i].y);

                        // Color has to be applied there to avoid blanking out cross sections
                        cl[i] = TrailColour;
                        uv2[i] = GetVertexUV2(TrailingEdgeType);
                    }
                }

                meshFiltersCtrlEdge[ctrlEdgeTypeInt].mesh.vertices = vp;
                meshFiltersCtrlEdge[ctrlEdgeTypeInt].mesh.uv = uv;
                meshFiltersCtrlEdge[ctrlEdgeTypeInt].mesh.uv2 = uv2;
                meshFiltersCtrlEdge[ctrlEdgeTypeInt].mesh.colors = cl;
                meshFiltersCtrlEdge[ctrlEdgeTypeInt].mesh.RecalculateBounds();
            }

            // Finally, simple top/bottom surface changes

            if (meshFilterCtrlSurface != null)
            {
                int length = meshReferenceCtrlSurface.vp.Length;
                Vector3[] vp = new Vector3[length];
                Array.Copy(meshReferenceCtrlSurface.vp, vp, length);
                Vector2[] uv = new Vector2[length];
                Array.Copy(meshReferenceCtrlSurface.uv, uv, length);
                Color[] cl = new Color[length];
                Vector2[] uv2 = new Vector2[length];

                for (int i = 0; i < vp.Length; ++i)
                {
                    // Span-based shift
                    if (vp[i].z < 0f)
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z + 0.5f - (float)Length / 2f);
                        uv[i] = new Vector2(0f, uv[i].y);
                    }
                    else
                    {
                        vp[i] = new Vector3(vp[i].x, vp[i].y, vp[i].z - 0.5f + (float)Length / 2f);
                        uv[i] = new Vector2((float)Length / 4f, uv[i].y);
                    }

                    // Width-based shift
                    if (vp[i].y < -0.1f)
                    {
                        if (vp[i].z < 0f)
                        {
                            vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)TipWidth, vp[i].z);
                            uv[i] = new Vector2(uv[i].x, (float)TipWidth / 4f);
                        }
                        else
                        {
                            vp[i] = new Vector3(vp[i].x, vp[i].y + 0.5f - (float)RootWidth, vp[i].z);
                            uv[i] = new Vector2(uv[i].x, (float)RootWidth / 4f);
                        }
                    }
                    else uv[i] = new Vector2(uv[i].x, 0f);

                    // Offsets & thickness
                    if (vp[i].z < 0f)
                    {
                        vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationTip, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetTipClamped);
                        uv[i] = new Vector2(uv[i].x + (vp[i].y * ctrlOffsetTipClamped) / 4f, uv[i].y);
                    }
                    else
                    {
                        vp[i] = new Vector3(vp[i].x * ctrlThicknessDeviationRoot, vp[i].y, vp[i].z + vp[i].y * ctrlOffsetRootClamped);
                        uv[i] = new Vector2(uv[i].x + (vp[i].y * ctrlOffsetRootClamped) / 4f, uv[i].y);
                    }

                    // Colors
                    if (vp[i].x > 0f)
                    {
                        cl[i] = TopColour;
                        uv2[i] = GetVertexUV2(SurfTopMat);
                    }
                    else
                    {
                        cl[i] = BottomColour;
                        uv2[i] = GetVertexUV2(SurfBottomMat);
                    }
                }
                meshFilterCtrlSurface.mesh.vertices = vp;
                meshFilterCtrlSurface.mesh.uv = uv;
                meshFilterCtrlSurface.mesh.uv2 = uv2;
                meshFilterCtrlSurface.mesh.colors = cl;
                meshFilterCtrlSurface.mesh.RecalculateBounds();
            }
            if (updateAerodynamics)
                CalculateAerodynamicValues();
        }

        #endregion

        #region Mesh Setup and Checking
        public override void SetupMeshFilters()
        {
            meshFilterCtrlFrame = CheckMeshFilter(meshFilterCtrlFrame, "frame");
            meshFilterCtrlSurface = CheckMeshFilter(meshFilterCtrlSurface, "surface");
            for (int i = 0; i < meshTypeCountEdgeCtrl; ++i)
            {
                MeshFilter meshFilterCtrlEdge = CheckMeshFilter("edge_type" + i);
                meshFiltersCtrlEdge.Add(meshFilterCtrlEdge);
            }
        }

        public override void SetupMeshReferences()
        {
            if (meshReferenceCtrlFrame == null || meshReferenceCtrlFrame.vp.Length == 0
                || meshReferenceCtrlSurface != null || meshReferenceCtrlSurface.vp.Length > 0
                || meshReferencesCtrlEdge[meshTypeCountEdgeCtrl - 1] != null || meshReferencesCtrlEdge[meshTypeCountEdgeCtrl - 1].vp.Length > 0)
            {
                meshReferenceCtrlFrame = FillMeshRefererence(meshFilterCtrlFrame);
                meshReferenceCtrlSurface = FillMeshRefererence(meshFilterCtrlSurface);
                for (int i = 0; i < meshTypeCountEdgeCtrl; ++i)
                {
                    MeshReference meshReferenceCtrlEdge = FillMeshRefererence(meshFiltersCtrlEdge[i]);
                    meshReferencesCtrlEdge.Add(meshReferenceCtrlEdge);
                }

            }
        }
        #endregion

        #region Materials
        public override void UpdateMaterials()
        {
            if (materialLayeredSurface == null || materialLayeredEdge == null)
                SetMaterialReferences();
            if (materialLayeredSurface != null)
            {
                SetMaterial(meshFilterCtrlSurface, materialLayeredSurface);
                SetMaterial(meshFilterCtrlFrame, materialLayeredEdge);
                for (int i = 0; i < meshTypeCountEdgeCtrl; ++i)
                {
                    SetMaterial(meshFiltersCtrlEdge[i], materialLayeredEdge);
                }
            }
        }

        // todo: this is all duplicate except the set textures call (which may be also a duplicate through *ctrlSurface vs *WingSurface
        public override void SetMaterialReferences()
        {
            if (materialLayeredSurface == null)
                materialLayeredSurface = new Material(StaticWingGlobals.B9WingShader);
            if (materialLayeredEdge == null)
                materialLayeredEdge = new Material(StaticWingGlobals.B9WingShader);

            SetTextures(meshFilterCtrlSurface, meshFilterCtrlFrame);

            if (materialLayeredSurfaceTextureMain != null && materialLayeredSurfaceTextureMask != null)
            {
                materialLayeredSurface.SetTexture("_MainTex", materialLayeredSurfaceTextureMain);
                materialLayeredSurface.SetTexture("_Emissive", materialLayeredSurfaceTextureMask);
                materialLayeredSurface.SetFloat("_Shininess", materialPropertyShininess);
                materialLayeredSurface.SetColor("_SpecColor", materialPropertySpecular);
            }

            if (materialLayeredEdgeTextureMain != null && materialLayeredEdgeTextureMask != null)
            {
                materialLayeredEdge.SetTexture("_MainTex", materialLayeredEdgeTextureMain);
                materialLayeredEdge.SetTexture("_Emissive", materialLayeredEdgeTextureMask);
                materialLayeredEdge.SetFloat("_Shininess", materialPropertyShininess);
                materialLayeredEdge.SetColor("_SpecColor", materialPropertySpecular);
            }
        }

        #endregion

        #region Aero

        // Aerodynamics value calculation
        // More or less lifted from pWings, so credit goes to DYJ and Taverius
        public override void CalculateAerodynamicValues()
        {
            CheckAssemblies();

            double sharedWidthTipSum = TipWidth + TipTrailingEdge;
            double sharedWidthRootSum = RootWidth + RootTrailingEdge;

            double ctrlOffsetRootLimit = (Length / 2f) / (RootWidth + RootTrailingEdge);
            double ctrlOffsetTipLimit = (Length / 2f) / (TipWidth + TipTrailingEdge);

            double ctrlOffsetRootClamped = Utils.Clamp(RootOffset, -ctrlOffsetRootLimit, ctrlOffsetRootLimit);
            double ctrlOffsetTipClamped = Utils.Clamp(TipOffset, -ctrlOffsetTipLimit, ctrlOffsetTipLimit);

            // Base four values
            double taperRatio = (Length + sharedWidthTipSum * ctrlOffsetTipClamped - sharedWidthRootSum * ctrlOffsetRootClamped) / Length;
            double MAC = (double)(sharedWidthTipSum + sharedWidthRootSum) / 2.0;
            double midChordSweep = Math.Atan((double)Math.Abs(sharedWidthRootSum - sharedWidthTipSum) / Length) * Utils.Rad2Deg;

            // Derived values

            double surfaceArea = MAC * Length;
            double aspectRatio = 2.0 * Length / MAC;

            ArSweepScale = Math.Pow(aspectRatio / Math.Cos(Utils.Deg2Rad * midChordSweep), 2.0f) + 4.0f;
            ArSweepScale = 2.0f + Math.Sqrt(ArSweepScale);
            ArSweepScale = (2.0f * Math.PI) / ArSweepScale * aspectRatio;

            wingMass = Utils.Clamp(massFudgeNumber * surfaceArea * ((ArSweepScale * 2.0) / (3.0 + ArSweepScale)) * ((1.0 + taperRatio) / 2), 0.01, double.MaxValue);
            Cd = dragBaseValue / ArSweepScale * dragMultiplier;
            Cl = liftFudgeNumber * surfaceArea * ArSweepScale;
            GatherChildrenCl();
            connectionForce = Math.Round(Utils.Clamp(Math.Sqrt(Cl + ChildrenCl) * (double)connectionFactor, (double)connectionMinimum, double.MaxValue));


            // Shared parameters

            updateCost();
            part.CoMOffset = new Vector3(0f, -(float)(sharedWidthRootSum + sharedWidthTipSum) / 4f, 0f);

            part.breakingForce = Mathf.Round((float)connectionForce);
            part.breakingTorque = Mathf.Round((float)connectionForce);

            // Stock-only values
            if (!FARactive)
            {
                float stockLiftCoefficient = (float)surfaceArea / 3.52f;
                ModuleControlSurface mCtrlSrf = part.Modules.GetModule<ModuleControlSurface>();
                mCtrlSrf.deflectionLiftCoeff = (float)Math.Round(stockLiftCoefficient, 2);
                mCtrlSrf.ctrlSurfaceArea = ctrlFraction;
                part.mass = stockLiftCoefficient * (1 + mCtrlSrf.ctrlSurfaceArea) * 0.1f;
            }
            else
                setFARModuleParams(midChordSweep, taperRatio, midChordOffsetFromOrigin);

            StartCoroutine(updateAeroDelayed());
        }

        public override void setFARModuleParams(double midChordSweep, double taperRatio, Vector3 midChordOffset)
        {
            base.setFARModuleParams(midChordSweep, taperRatio, midChordOffset);
            if (aeroFARFieldInfoControlSurfaceFraction != null)
            {
                aeroFARFieldInfoControlSurfaceFraction.SetValue(aeroFARModuleReference, ctrlFraction);
                aeroFARMethodInfoUsed.Invoke(aeroFARModuleReference, null);
            }
        }


        #endregion

        #region Alternative UI/input


        public override string WindowTitle
        {
            get
            {
                return "Control surface";
            }
        }

        public override void ShowEditorUI()
        {
            base.ShowEditorUI();

            WindowManager.Window.FindPropertyGroup("Base").UpdatePropertyValues(length, rootWidth, tipWidth, tipOffset, rootThickness, tipThickness, rootOffset);
            WindowManager.Window.FindPropertyGroup("Edge (leading)").UpdatePropertyValues(leadingEdgeType, rootLeadingEdge, tipLeadingEdge);
            WindowManager.Window.FindPropertyGroup("Edge (trailing)").UpdatePropertyValues(trailingEdgeType, rootTrailingEdge, tipTrailingEdge);
            WindowManager.Window.FindPropertyGroup("Surface (top)").UpdatePropertyValues(surfTopMat, surfTopOpacity, surfTopHue, surfTopSat, surfTopBright);
            WindowManager.Window.FindPropertyGroup("Surface (bottom)").UpdatePropertyValues(surfBottomMat, surfBottomOpacity, surfBottomHue, surfBottomSat, surfBottomBright);
            WindowManager.Window.FindPropertyGroup("Surface (leading edge)").UpdatePropertyValues(surfLeadMat, surfLeadOpacity, surfLeadHue, surfLeadSat, surfLeadBright);
            WindowManager.Window.FindPropertyGroup("Surface (trailing edge)").UpdatePropertyValues(surfTrailMat, surfTrailOpacity, surfTrailHue, surfTrailSat, surfTrailBright);
        }

        public override UI.EditorWindow CreateWindow()
        {
            UI.EditorWindow window = new EditorWindow();
            
            PropertyGroup basegroup = window.AddPropertyGroup("Base", new Color(0.25f, 0.5f, 0.4f, 1f));
            basegroup.AddProperty(new WingProperty(length), x => window.wing.Length = x);
            basegroup.AddProperty(new WingProperty(rootWidth), x => window.wing.RootWidth = x);
            basegroup.AddProperty(new WingProperty(tipWidth), x => window.wing.TipWidth = x);
            basegroup.AddProperty(new WingProperty(rootOffset), x => ((B9_ProceduralControl)window.wing).RootOffset = x);
            basegroup.AddProperty(new WingProperty(tipOffset), x => window.wing.TipOffset = x);
            basegroup.AddProperty(new WingProperty(rootThickness), x => window.wing.RootThickness = x);
            basegroup.AddProperty(new WingProperty(tipThickness), x => window.wing.TipThickness = x);

            UI.PropertyGroup leadgroup = window.AddPropertyGroup("Edge (leading)", UIUtility.ColorHSBToRGB(uiColorSliderEdgeL));
            leadgroup.AddProperty(new WingProperty(leadingEdgeType), x => ((B9_ProceduralWing)window.wing).LeadingEdgeType = (int)x,
                                new string[] { "Uniform", "Standard", "Reinforced", "LRSI", "HRSI" });
            leadgroup.AddProperty(new WingProperty(rootLeadingEdge), x => ((B9_ProceduralWing)window.wing).RootLeadingEdge = x);
            leadgroup.AddProperty(new WingProperty(tipLeadingEdge), x => ((B9_ProceduralWing)window.wing).TipLeadingEdge = x);

            UI.PropertyGroup trailGroup = window.AddPropertyGroup("Edge (trailing)", UIUtility.ColorHSBToRGB(uiColorSliderEdgeT));
            trailGroup.AddProperty(new WingProperty(leadingEdgeType), x => ((B9_ProceduralWing)window.wing).TrailingEdgeType = (int)x);
            trailGroup.AddProperty(new WingProperty(rootLeadingEdge), x => ((B9_ProceduralWing)window.wing).RootTrailingEdge = x);
            trailGroup.AddProperty(new WingProperty(tipLeadingEdge), x => ((B9_ProceduralWing)window.wing).TipTrailingEdge = x);

            UI.PropertyGroup surfTGroup = window.AddPropertyGroup("Surface (top)", UIUtility.ColorHSBToRGB(uiColorSliderColorsST));
            surfTGroup.AddProperty(new WingProperty(surfTopMat), x => ((B9_ProceduralWing)window.wing).SurfTopMat = (int)x);
            surfTGroup.AddProperty(new WingProperty(surfTopOpacity), x => ((B9_ProceduralWing)window.wing).SurfTopOpacity = x);
            surfTGroup.AddProperty(new WingProperty(surfTopHue), x => ((B9_ProceduralWing)window.wing).SurfTopHue = x);
            surfTGroup.AddProperty(new WingProperty(surfTopSat), x => ((B9_ProceduralWing)window.wing).SurfTopSat = x);
            surfTGroup.AddProperty(new WingProperty(surfTopBright), x => ((B9_ProceduralWing)window.wing).SurfTopBright = x);

            UI.PropertyGroup surfBGroup = window.AddPropertyGroup("Surface (bottom)", UIUtility.ColorHSBToRGB(uiColorSliderColorsSB));
            surfBGroup.AddProperty(new WingProperty(surfBottomMat), x => ((B9_ProceduralWing)window.wing).SurfBottomMat = (int)x);
            surfBGroup.AddProperty(new WingProperty(surfBottomOpacity), x => ((B9_ProceduralWing)window.wing).SurfBottomOpacity = x);
            surfBGroup.AddProperty(new WingProperty(surfBottomHue), x => ((B9_ProceduralWing)window.wing).SurfBottomHue = x);
            surfBGroup.AddProperty(new WingProperty(surfBottomSat), x => ((B9_ProceduralWing)window.wing).SurfBottomSat = x);
            surfBGroup.AddProperty(new WingProperty(surfBottomBright), x => ((B9_ProceduralWing)window.wing).SurfBottomBright = x);

            UI.PropertyGroup surfLGroup = window.AddPropertyGroup("Surface (leading edge)", UIUtility.ColorHSBToRGB(uiColorSliderColorsEL));
            surfLGroup.AddProperty(new WingProperty(surfLeadMat), x => ((B9_ProceduralWing)window.wing).SurfLeadMat = (int)x);
            surfLGroup.AddProperty(new WingProperty(surfLeadOpacity), x => ((B9_ProceduralWing)window.wing).SurfLeadOpacity = x);
            surfLGroup.AddProperty(new WingProperty(surfLeadHue), x => ((B9_ProceduralWing)window.wing).SurfLeadHue = x);
            surfLGroup.AddProperty(new WingProperty(surfBottomSat), x => ((B9_ProceduralWing)window.wing).SurfLeadSat = x);
            surfLGroup.AddProperty(new WingProperty(surfLeadBright), x => ((B9_ProceduralWing)window.wing).SurfLeadBright = x);

            UI.PropertyGroup surfRGroup = window.AddPropertyGroup("Surface (trailing edge)", UIUtility.ColorHSBToRGB(uiColorSliderColorsET));
            surfRGroup.AddProperty(new WingProperty(surfTrailMat), x => ((B9_ProceduralWing)window.wing).SurfTrailMat = (int)x);
            surfRGroup.AddProperty(new WingProperty(surfTrailOpacity), x => ((B9_ProceduralWing)window.wing).SurfTrailOpacity = x);
            surfRGroup.AddProperty(new WingProperty(surfTrailHue), x => ((B9_ProceduralWing)window.wing).SurfTrailHue = x);
            surfRGroup.AddProperty(new WingProperty(surfTrailSat), x => ((B9_ProceduralWing)window.wing).SurfTrailSat = x);
            surfRGroup.AddProperty(new WingProperty(surfTrailBright), x => ((B9_ProceduralWing)window.wing).SurfTrailBright = x);

            return window;
        }

        #endregion
    }
}