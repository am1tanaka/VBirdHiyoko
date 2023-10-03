using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// 様々なクラスのインスタンスを型で登録して、
    /// システムで共有するのに使うためのクラス。
    /// </summary>
    public class InstanceDictionary
    {
        readonly Dictionary<System.Type, object> instances = new Dictionary<System.Type, object>();

        /// <summary>
        /// 指定の方のインスタンスを登録する。
        /// 同じ型が登録済みの時は登録せずに false を返す。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="instance">インスタンス</param>
        /// <returns>登録できたとき、true。すでに登録済みの時 false</returns>
        public bool Register<T>(T instance)
        {
            if (instances.ContainsKey(instance.GetType()))
            {
                return false;
            }
            instances[instance.GetType()] = instance;
            return true;
        }

        /// <summary>
        /// 型を指定して、登録されているインスタンスを取り出す。
        /// </summary>
        /// <typeparam name="T">取り出したいクラス</typeparam>
        /// <returns>指定のクラスで登録されていたインスタンスを返す。未登録の時は、該当クラスを生成して、生成したインスタンスを返す。</returns>
        public T Get<T>() where T : class, new()
        {
            if (!instances.ContainsKey(typeof(T)))
            {
                instances[typeof(T)] = new T();
            }

            return instances[typeof(T)] as T;
        }
    }
}