using System;
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
            if (instances.ContainsKey(typeof(T)))
            {
                return false;
            }

            instances[typeof(T)] = instance;
            return true;
        }

        /// <summary>
        /// 指定した型で登録されているインスタンスを返す。
        /// </summary>
        /// <typeparam name="T">取り出したいクラス</typeparam>
        /// <returns>指定のクラスで登録されていたインスタンスを返す。未登録の時は、null</returns>
        public T Get<T>()
        {
            if (!instances.ContainsKey(typeof(T)))
            {
                return default(T);
            }

            return (T)instances[typeof(T)];
        }
    }
}