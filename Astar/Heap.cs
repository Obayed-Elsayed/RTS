using System;
using System.Collections;
using UnityEngine;

public class Heap<T> where T: IHeapitem<T>{
    T[] items;
    int currentItemCount;


    public Heap(int maxHeapSize){
        items = new T[maxHeapSize];
    }

    public void Add(T item){
            // let the item know its index in the heap
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            sortUp(item);
            currentItemCount++;
    }

    public T removeFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].HeapIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

    void SortDown(T item) {
		while (true) {
			int childLeft = item.HeapIndex * 2 + 1;
			int childRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			if (childLeft < currentItemCount) {
				swapIndex = childLeft;

				if (childRight < currentItemCount) {
					if (items[childLeft].CompareTo(items[childRight]) < 0) {
						swapIndex = childRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
				}
				else {
					return;
                }

			}
			else {
                // no children, can just exit
				return;
			}

		}
	}

    void sortUp(T item) {
        // parent index formula for a heap
        int parentIndex = (item.HeapIndex-1)/2;
        while(true) {
            T parentitem = items[parentIndex];
            if (item.CompareTo(parentitem) > 0) {
                Swap(item, parentitem);
            }else{
                break;
            }
            parentIndex = (item.HeapIndex-1)/2;
        }
    }

    void Swap(T t1, T t2){
        items[t1.HeapIndex] = t2;
        items[t2.HeapIndex] = t1;
        int t1Index = t1.HeapIndex;
        t1.HeapIndex = t2.HeapIndex;
        t2.HeapIndex = t1Index;
    }

    public int Count {
		get {
			return currentItemCount;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.HeapIndex], item);
	}

    public void UpdateItem(T item){
        sortUp(item);
    }
}

// Icompareable -> can compare objects
// same priority = 0 higher priority =1, lower priority = -1
public interface IHeapitem<T>: IComparable<T>{
    int HeapIndex{
        get;
        set;
    }
}
