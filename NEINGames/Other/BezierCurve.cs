using UnityEngine;

namespace NEINGames.BezierCurve {

    public class BezierCurve2D {
        Vector2 _p0;
        Vector2 _p1;
        Vector2? _p2;
        Vector2? _p3;
        CurveType _type;

        public Vector2 StartPoint{get => _p0;}
        public Vector2 EndPoint
        {
            get 
            {
                if (_p2 == null)
                    return _p1;
                else if (_p3 == null)
                    return (Vector2) _p2;
                else
                    return (Vector2) _p3;
            }
        }

        public Vector2? this[int id]
        {
            get 
            {
                switch(id)
                {
                    case 0:
                    return _p0;
                    case 1:
                    return _p1;
                    case 2:
                    return _p2;
                    case 3:
                    return _p3;
                    case -1:
                    if (_p2 == null)
                        return _p1;
                    else if (_p3 == null)
                        return _p2;
                    else
                        return _p3;
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

        public BezierCurve2D(Vector2 p0, Vector2 p1, Vector2? p2 = null, Vector2? p3 = null) {
            Set(p0,p1,p2,p3);
        }

        public void Set(Vector2 p0, Vector2 p1, Vector2? p2 = null, Vector2? p3 = null) {
            _p0 = p0;
            _p1 = p1;
            _p2 = p2;
            _p3 = p3;

            if (p2 == null)
                _type = CurveType.LINEAR;
            else if (p3 == null)
                _type = CurveType.QUADRATIC;
            else
                _type = CurveType.CUBE;
        }

        public Vector2 GetPoint(float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            
            switch (_type)
            {
                case CurveType.LINEAR:
                return oneMinusT * _p0 + t * _p1;

                case CurveType.QUADRATIC:
                return
                    Mathf.Pow(oneMinusT, 2) * _p0 +
                    2f * oneMinusT * t * _p1 +
                    Mathf.Pow(t, 2) * (Vector2) _p2;

                case CurveType.CUBE:
                return
                    Mathf.Pow(oneMinusT, 3) * _p0 +
                    3f * Mathf.Pow(oneMinusT, 2) * t * _p1 +
                    3f * oneMinusT * Mathf.Pow(t, 2) * (Vector2) _p2 +
                    Mathf.Pow(t, 3) * (Vector2) _p3;

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
                    2f * (1f - t) * (_p1 - _p0) +
			        2f * t * ((Vector2) _p2 - _p1);

                case CurveType.CUBE:
                return
                    3f * Mathf.Pow(oneMinusT, 2) * (_p1 - _p0) +
			        6f * oneMinusT * t * ((Vector2) _p2 - _p1) +
			        3f * Mathf.Pow(t, 2) * ((Vector2) _p3 - (Vector2) _p2);

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
            Debug.DrawLine(_p0,_p1, color, duration);

            if (_p2 != null)
                Debug.DrawLine(_p1,(Vector2) _p2, color, duration);
            if (_p3 != null)
                Debug.DrawLine((Vector2) _p2,(Vector2) _p3, color, duration);
        }

        void DrawCurve(float increment, float duration)
        {
            if (_type.Equals(CurveType.LINEAR))
                return;

            var color = Color.red;
            var priorP = _p0;

            for (float t = 0; t <= 1; t += increment)
            {   
                var nextP = GetPoint(t);
                Debug.DrawLine(priorP,nextP, color, duration);
                priorP = nextP;
            }
        }
    }
}