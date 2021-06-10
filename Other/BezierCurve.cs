using UnityEngine;

// TODO
// Improve to achieve constant segment length: https://www.habrador.com/tutorials/interpolation/3-move-along-curve/

namespace NEINGames.BezierCurve {

    public class BezierCurve2D 
    {
        public Vector2 P0{get; private set;}
        public Vector2 P1{get; private set;}
        public Vector2? P2{get; private set;}
        public Vector2? P3{get; private set;}
        CurveType _type;

        public Vector2 StartPoint{get => P0;}
        public Vector2 MiddlePoint{get => GetPoint(0.5f);}
        public Vector2 EndPoint{get => GetPoint(1);}

        public Vector2? this[int id]
        {
            get 
            {
                switch(id)
                {
                    case 0:
                    return P0;
                    case 1:
                    return P1;
                    case 2:
                    return P2;
                    case 3:
                    return P3;
                    case -1:
                    if (P2 == null)
                        return P1;
                    else if (P3 == null)
                        return P2;
                    else
                        return P3;
                    default:
                    return null;
                }
            }
        }

        enum CurveType {
            LINEAR,
            QUADRATIC,
            CUBE
        }

        public BezierCurve2D(Vector2 p0, Vector2 p1, Vector2? p2 = null, Vector2? p3 = null) 
        {
            Set(p0,p1,p2,p3);
        }

        public void Set(Vector2 p0, Vector2 p1, Vector2? p2 = null, Vector2? p3 = null) 
        {
            P0 = p0;
            P1 = p1;
            P2 = p2;
            P3 = p3;

            if (p2 == null)
                _type = CurveType.LINEAR;
            else if (p3 == null)
                _type = CurveType.QUADRATIC;
            else
                _type = CurveType.CUBE;
        }

        public Vector2 GetPoint(float t) 
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            
            switch (_type)
            {
                case CurveType.LINEAR:
                return oneMinusT * P0 + t * P1;

                case CurveType.QUADRATIC:
                return
                    Mathf.Pow(oneMinusT, 2) * P0 +
                    2f * oneMinusT * t * P1 +
                    Mathf.Pow(t, 2) * (Vector2) P2;

                case CurveType.CUBE:
                return
                    Mathf.Pow(oneMinusT, 3) * P0 +
                    3f * Mathf.Pow(oneMinusT, 2) * t * P1 +
                    3f * oneMinusT * Mathf.Pow(t, 2) * (Vector2) P2 +
                    Mathf.Pow(t, 3) * (Vector2) P3;

                default:
                return Vector2.zero;
            }
        }

        public Vector2 GetFirstDerivative(float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;

            switch (_type)
            {
                case CurveType.LINEAR:
                return Vector2.zero; // TODO

                case CurveType.QUADRATIC:
                return
                    2f * (1f - t) * (P1 - P0) +
			        2f * t * ((Vector2) P2 - P1);

                case CurveType.CUBE:
                return
                    3f * Mathf.Pow(oneMinusT, 2) * (P1 - P0) +
			        6f * oneMinusT * t * ((Vector2) P2 - P1) +
			        3f * Mathf.Pow(t, 2) * ((Vector2) P3 - (Vector2) P2);

                default:
                return Vector2.zero;
            }
        }

        // TODO Experiment with Gizmo-drawing.
        public void Draw(float increment = 0.1f, float duration = 10f)
        {
            DrawPoints(duration);
            DrawCurve(increment, duration);
        }
        

        void DrawPoints(float duration)
        {
            var color = Color.green;
            Debug.DrawLine(P0,P1, color, duration);

            if (P2 != null)
                Debug.DrawLine(P1,(Vector2) P2, color, duration);
            if (P3 != null)
                Debug.DrawLine((Vector2) P2,(Vector2) P3, color, duration);
        }

        void DrawCurve(float increment, float duration)
        {
            if (_type.Equals(CurveType.LINEAR))
                return;

            var color = Color.red;
            var priorP = P0;

            for (float t = 0; t <= 1; t += increment)
            {   
                var nextP = GetPoint(t);
                Debug.DrawLine(priorP,nextP, color, duration);
                priorP = nextP;
            }
        }
    }
}