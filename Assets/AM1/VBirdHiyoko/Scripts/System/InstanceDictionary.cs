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
        /// 指定の型の登録を削除する。
        /// </summary>
        /// <typeparam name="T">削除したい型</typeparam>
        public void Unregister<T>()
        {
            if (instances.ContainsKey(typeof(T)))
            {
                instances.Remove(typeof(T));
            }
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

        /// <summary>
        /// 指定の型のインスタンスを取り出す。
        /// 指定の型が未登録なら生成して、登録したものを返す。
        /// </summary>
        /// <typeparam name="T">型指定</typeparam>
        /// <returns>取り出すか生成したインスタンス</returns>
        public T GetOrNew<T>() where T : new()
        {
            if (!instances.ContainsKey(typeof(T)))
            {
                Register<T>(new T());
            }

            return (T)instances[typeof(T)];
        }
    }
}