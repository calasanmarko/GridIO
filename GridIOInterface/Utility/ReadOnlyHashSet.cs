using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GridIOInterface {
    public class ReadOnlyHashSet<T> : IEnumerable<T> {
        private HashSet<T> hashSet;

        public T this[T t] {
            get {
                if (hashSet.TryGetValue(t, out T res)) {
                    return res;
                }
                else {
                    return default;
                }
            }
        }

        public T First {
            get {
                foreach (T t in hashSet) {
                    return t;
                }
                return default;
            }
        }

        public ReadOnlyHashSet() {
            this.hashSet = new HashSet<T>();
        }

        public ReadOnlyHashSet(HashSet<T> hashSet) {
            this.hashSet = hashSet;
        }

        public int Count {
            get {
                return hashSet.Count;
            }
        }

        public bool Contains(T t) {
            return hashSet.Contains(t);
        }

        public IEnumerator<T> GetEnumerator() {
            return hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return hashSet.GetEnumerator();
        }

        public static explicit operator ReadOnlyHashSet<T>(HashSet<T> hashSet) {
            return new ReadOnlyHashSet<T>(hashSet);
        }
    }
}
