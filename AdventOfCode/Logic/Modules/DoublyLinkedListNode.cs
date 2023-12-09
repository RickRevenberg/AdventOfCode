namespace AdventOfCode.Logic.Modules
{
    public class DoublyLinkedListNode<T>
    {
        internal int TrackingId { get; init; }

        internal DoublyLinkedListNode<T> Previous { get; set; }
        internal DoublyLinkedListNode<T> Next { get; set; }

        internal T Value { get; set; }
    }
}
