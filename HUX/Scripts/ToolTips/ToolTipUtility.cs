﻿using System;
using UnityEngine;

public static class ToolTipUtility {

    /// <summary>
    /// Used to find a pivot point that is closest to the 
    /// anchor. This ensures a natural-looking attachment where the connector line
    /// meets the label.
    /// </summary>

    const int NumPivotLocations = 8;

    [Serializable]
    public enum AttachPointTypeEnum
    {
        // Specific options
        // These double as array positions
        BotMiddle = 0,
        TopMiddle = 1,
        RightMiddle = 2,
        LeftMiddle = 3,
        BotRightCorner = 4,
        BotLeftCorner = 5,
        TopRightCorner = 6,
        TopLeftCorner = 7,
        // Automatic options
        Center,
        Closest,
        ClosestMiddle,
        ClosestCorner,
        // Smoothly interpolate between positions
        // (UNIMPLEMENTED)
        //Continuous,
    }

    //Note: Avoid running this query in Update function because calculating Vector3.Distance requires sqr root calculation (expensive)
    //Instead, find strategic moments to update nearest pivot (i.e. only once when ToolTip becomes active)
    public static Vector3 FindClosestAttachPointToAnchor(Transform anchor, Transform contentParent, Vector3[] localPivotPositions, AttachPointTypeEnum pivotType)
    {
        Vector3 nearPivot = Vector3.zero;
        Vector3 currentPivot = Vector3.zero;
        Vector3 anchorPosition = anchor.position;
        float nearDist = Mathf.Infinity;

        if (localPivotPositions == null || localPivotPositions.Length < NumPivotLocations)
            return nearPivot;
        
        switch (pivotType) {

            case AttachPointTypeEnum.Center:
                return nearPivot;

                // Search all pivots
            case AttachPointTypeEnum.Closest:
                for (int i = 0; i < localPivotPositions.Length; i++) {
                    currentPivot = localPivotPositions[i];
                    float dist = Vector3.Distance(anchorPosition, contentParent.TransformPoint(currentPivot));
                    if (dist < nearDist) {
                        nearDist = dist;
                        nearPivot = currentPivot;
                    }
                }
                break;

                // Search corner pivots
            case AttachPointTypeEnum.ClosestCorner:
                for (int i = (int)AttachPointTypeEnum.BotRightCorner; i < (int)AttachPointTypeEnum.TopLeftCorner; i++) {
                    currentPivot = localPivotPositions[i];
                    float dist = Vector3.Distance(anchorPosition, contentParent.TransformPoint(currentPivot));
                    if (dist < nearDist) {
                        nearDist = dist;
                        nearPivot = currentPivot;
                    }
                }
                break;

                // Search middle pivots
            case AttachPointTypeEnum.ClosestMiddle:
                for (int i = (int)AttachPointTypeEnum.BotMiddle; i < (int)AttachPointTypeEnum.LeftMiddle; i++) {
                    currentPivot = localPivotPositions[i];
                    float dist = Vector3.Distance(anchorPosition, contentParent.TransformPoint(currentPivot));
                    if (dist < nearDist) {
                        nearDist = dist;
                        nearPivot = currentPivot;
                    }
                }
                break;

            default:
                // For all other types, just use the array position
                // TODO error checking for array size (?)
                nearPivot = localPivotPositions[(int)pivotType];
                break;
        }

        return nearPivot;
    }

    public static void GetAttachPointPositions (ref Vector3[] pivotPositions, Vector2 localContentSize) {
        if (pivotPositions == null || pivotPositions.Length < NumPivotLocations)
            pivotPositions = new Vector3[NumPivotLocations];

        //Get the extents of our content size
        localContentSize *= 0.5f;

        pivotPositions[(int)AttachPointTypeEnum.BotMiddle]        = new Vector3(0f, -localContentSize.y, 0f);
        pivotPositions[(int)AttachPointTypeEnum.TopMiddle]        = new Vector3(0f, localContentSize.y, 0f);
        pivotPositions[(int)AttachPointTypeEnum.LeftMiddle]      = new Vector3(-localContentSize.x, 0f, 0f); // was right
        pivotPositions[(int)AttachPointTypeEnum.RightMiddle]       = new Vector3(localContentSize.x, 0f, 0f); // was left

        pivotPositions[(int)AttachPointTypeEnum.BotLeftCorner]   = new Vector3(-localContentSize.x, -localContentSize.y, 0f); // was right
        pivotPositions[(int)AttachPointTypeEnum.BotRightCorner]    = new Vector3(localContentSize.x, -localContentSize.y, 0f); // was left
        pivotPositions[(int)AttachPointTypeEnum.TopLeftCorner]   = new Vector3(-localContentSize.x, localContentSize.y, 0f); // was right
        pivotPositions[(int)AttachPointTypeEnum.TopRightCorner]    = new Vector3(localContentSize.x, localContentSize.y, 0f); // was left
    }
}
