using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversatioQueue
{
    private Queue<Convesation> convesationsQueue=new Queue<Convesation>();
    public Convesation top() { return convesationsQueue.Peek();}

    public void Enqueue(Convesation con) { convesationsQueue.Enqueue(con); }
    public void EnqueuePriority(Convesation con) 
    { 
        Queue<Convesation>queue=new Queue<Convesation>();
        queue.Enqueue(con);
        while (convesationsQueue.Count > 0)
        {
                queue.Enqueue(convesationsQueue.Dequeue());
        }
        convesationsQueue = queue;
    }
    public void Dequeue() 
    {
        if (convesationsQueue.Count > 0)
        {
            convesationsQueue.Dequeue();
        }
    }
    public bool IsEmpty() { return convesationsQueue.Count == 0; }
}
