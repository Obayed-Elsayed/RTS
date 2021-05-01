using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
public class PathFinderManager : MonoBehaviour
{
    private Queue<PathResult> results = new Queue<PathResult>();
    public static PathFinderManager manager;

    PathRequest currentRequest;    

    PathFinder pathFinder;

    public void Awake(){
        manager = this;
        pathFinder = GetComponent<PathFinder>();
    }

    public void Update() {
        if(results.Count > 0){
            int queuelength = results.Count;
            lock(results){
                for(int i = 0; i < queuelength; i++){
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }
    }
    public static void requestPath(PathRequest request){
        ThreadStart threadStart = delegate{
            manager.pathFinder.FindPath(request, manager.finishedPathFinding);
        };
        threadStart.Invoke();
    }

    public void finishedPathFinding(PathResult result){
        lock(results){
            results.Enqueue(result);
        }
    }

}
public struct PathRequest {
        public Vector3 start;
        public Vector3 dest;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 start, Vector3 dest, Action<Vector3[], bool> callback) {
            this.start = start;
            this.dest = dest;
            this.callback = callback;
        }
}
public struct PathResult {
        public Vector3[] path;
        public bool success;
        public Action<Vector3[], bool> callback;

        public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback) {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }
}


