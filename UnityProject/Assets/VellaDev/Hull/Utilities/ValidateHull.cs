﻿using Unity.Mathematics;
using UnityEngine;

namespace VellaDev.Hull
{
    public static class ValidateHull
    {
        public static unsafe void Validate(this NativeHull hull)
        {
            Debug.Assert(hull.FaceCount > 0);
            Debug.Assert(hull.EdgeCount > 0);

            for (int i = 0; i < hull.FaceCount; ++i)
            {
                ValidateFace(hull, hull.Faces + i);
            }
        }

        public static unsafe void ValidateFace(this NativeHull hull, NativeFace* face)
        {
            Debug.Assert(hull.FaceCount > 0);
            Debug.Assert(hull.EdgeCount > 0);
            Debug.Assert(face->Edge != -1);

            ValidateEdge(hull, hull.edges + face->Edge);
        }

        public static unsafe void ValidateEdge(this NativeHull hull, NativeHalfEdge* edge)
        {
            Debug.Assert(hull.FaceCount > 0);
            Debug.Assert(hull.EdgeCount > 0);
            Debug.Assert(edge->Twin != -1);

            NativeHalfEdge* curTwin = hull.edges + edge->Twin;

            int edgeIndex = (int)(edge - hull.edges);

            Debug.Assert(curTwin->Twin == edgeIndex, "The twin of the edge twin must be the edge itself");
            Debug.Assert(math.abs(edge->Twin - edgeIndex) == 1, "The two edges must be close by one index.");
            Debug.Assert(hull.edges[edge->Prev].Next == edgeIndex, "The twin of the edge twin must be the edge");
            Debug.Assert(edge->Origin != curTwin->Origin, "Edges and their twin must point to each others' origin vertex");

            int count = 0;
            NativeHalfEdge* start = edge;
            do
            {
                NativeHalfEdge* next = hull.edges + edge->Next;
                NativeHalfEdge* twin = hull.edges + next->Twin;
                edge = twin;

                Debug.Assert(edge->Face != -1, "All edges must have a face index");

                bool infiniteLoop = count > hull.EdgeCount;
                if (count > hull.EdgeCount)
                {
                    Debug.Assert(true, "Possible infinite Edge Loop");
                    break;
                }
                ++count;
            }
            while (edge != start);

        }
    }
}

