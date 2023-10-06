//#define DEBUG_LOG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AM1.VBirdHiyoko
{
    /// <summary>
    /// ルートを探索するためのクラス。
    /// </summary>
    public class Route
    {
        /// <summary>
        /// 当たり判定の余白
        /// </summary>
        public static float Margin => 0.005f;

        /// <summary>
        /// プレイヤーが立っているブロック
        /// </summary>
        public BlockRouteData RootBlock { get; private set; }

        /// <summary>
        /// 探索済みフラグ
        /// </summary>
        public bool IsSearched { get; private set; }

        /// <summary>
        /// プレイ継続できる時、true
        /// </summary>
        public bool CanPlay { get; private set; }

        /// <summary>
        /// ブロック用レイヤー
        /// </summary>
        static readonly int blockLayer;

        static int BlockListMax => 100;
        static List<BlockRouteData> collideBlockList = new List<BlockRouteData>(BlockListMax);

        /// <summary>
        /// 接触確認用配列
        /// </summary>
        static Collider[] results = new Collider[8];

        /// <summary>
        /// Raycastの結果を受け取る配列
        /// </summary>
        static RaycastHit[] raycastHits = new RaycastHit[8];

        /// <summary>
        /// 探索用データ
        /// </summary>
        RouteSearchList routeSearchList = new RouteSearchList();

        static Route()
        {
            blockLayer = LayerMask.GetMask("Block");
        }

        /// <summary>
        /// プレイヤーの足元のブロックを受け取って探索開始。
        /// </summary>
        /// <param name="rootBlock">プレイヤーの足元のブロック</param>
        /// <param name="playerCollider">プレイヤーの当たり判定</param>
        /// <param name="stepMax">越えられる段差の高さ</param>
        public void Search(BlockRouteData rootBlock, Collider playerCollider, float stepMax)
        {
            // 探査済みなら何もしない
            if (IsSearched) return;

            // 探索データを削除
            IsSearched = true;
            RootBlock = rootBlock;
            CanPlay = false;
            collideBlockList.Clear();

            // 調査元の設定
            routeSearchList.AddFirstBlock(RootBlock);

            // リストを処理
            BlockRouteData block;
            int count = 0;
            while ((block = routeSearchList.Get()) != null)
            {
                VBirdHiyokoManager.Log($"[{count}]---- {routeSearchList.ListCount}");
                CheckAndAddSearchList(block, Direction.Type.Forward, playerCollider, stepMax);
                CheckAndAddSearchList(block, Direction.Type.Right, playerCollider, stepMax);
                CheckAndAddSearchList(block, Direction.Type.Back, playerCollider, stepMax);
                CheckAndAddSearchList(block, Direction.Type.Left, playerCollider, stepMax);
                count++;
                if (count > 1000)
                {
                    Debug.Log($"over");
                    IsSearched = false;
                    break;
                }
            }

            // 詰みチェック
            StuckCheck();
        }

        /// <summary>
        /// 指定のブロックの指定の方向にブロックがあるかどうかを調べて、あればリストに登録する。
        /// </summary>
        /// <param name="fromBlock">探索元のブロック</param>
        /// <param name="dir">探索する方向</param>
        void CheckAndAddSearchList(BlockRouteData fromBlock, Direction.Type dir, Collider playerCollider, float stepMax)
        {
            float centerY = fromBlock.TopY + playerCollider.bounds.extents.y + Margin;
            Vector3 center = fromBlock.transform.position + centerY * Vector3.up + Direction.Vector[(int)dir];
            int count = Physics.OverlapBoxNonAlloc(
                center, playerCollider.bounds.extents, results, Quaternion.identity, blockLayer);

            if (count > 0)
            {
                // 接触あり。登りか押す
                VBirdHiyokoManager.Log($"接触有り。登りか押せるか {center}");
                CollideBlock(fromBlock, dir, count, stepMax);
            }
            else
            {
                // 接触無し。足場の確認
                VBirdHiyokoManager.Log($"接触なし。足場チェック {center}");
                SearchFloor(center, fromBlock, dir);
            }
        }

        /// <summary>
        /// 詰みチェック
        /// </summary>
        void StuckCheck()
        {
            // すでにゴールにたどり着いているなら詰みではない
            if (CanPlay) return;

            for (int i = 0; i < collideBlockList.Count; i++)
            {
                var block = collideBlockList[i];
                block.CheckPushDirectionAndNextToThePlayer();

                // 前後両方とも空いているか、プレイヤーが前後どちらかの隣の時、前後の一方でも歩ければプレイ継続可能
                // 左右両方とも空いていてか、プレイヤーが左右どちらかの隣の時、左右の一方でも歩ければプレイ継続可能
                if (CanPushFromNext(block)
                    || (CanXPush(block) && CanXWalk(block))
                    || (CanZPush(block) && CanZWalk(block)))
                {
                    CanPlay = true;
                    return;
                }
            }
        }

        /// <summary>
        /// 足場を確認
        /// </summary>
        /// <param name="fromBlock">確認元のブロック</param>
        /// <param name="dir">確認方向</param>
        void SearchFloor(Vector3 center, BlockRouteData fromBlock, Direction.Type dir)
        {
            int count = Physics.RaycastNonAlloc(center, Vector3.down, raycastHits, float.MaxValue, blockLayer);
            // 足場がなければ移動なし
            if (count == 0)
            {
                VBirdHiyokoManager.Log($"足場なし");
                return;
            }

            // 一番上の高さを調べる
            float floorTop = raycastHits[0].distance;
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                if (raycastHits[i].distance < floorTop)
                {
                    floorTop = raycastHits[i].distance;
                    index = i;
                }
            }

            var floorBlock = raycastHits[index].collider.GetComponent<BlockRouteData>();
            if (floorBlock == null) return;
            fromBlock.SetLinkedBlock(dir, floorBlock);
            routeSearchList.Add(fromBlock, floorBlock, Direction.Reverse(dir));
            if (floorBlock.CompareTag("Goal"))
            {
                VBirdHiyokoManager.Log($"ゴールが歩ける {floorBlock.transform.position}");
                CanPlay = true;
            }

            VBirdHiyokoManager.Log($"足場あり {floorBlock.transform.position}");
        }

        /// <summary>
        /// 接触するブロックの処理
        /// </summary>
        /// <param name="fromBlock">調査元ブロック</param>
        /// <param name="count">resultsの個数</param>
        /// <param name="stepMax">段差上限</param>
        void CollideBlock(BlockRouteData fromBlock, Direction.Type dir, int count, float stepMax)
        {
            var topBlock = GetHighestCollider(count);

            if (topBlock.bounds.max.y - fromBlock.TopY <= stepMax)
            {
                // 歩ける
                VBirdHiyokoManager.Log($"歩ける fromY={fromBlock.TopY} topY={topBlock.bounds.max.y} {topBlock.bounds.center}");
                var topBlockData = topBlock.GetComponent<BlockRouteData>();
                fromBlock.SetLinkedBlock(dir, topBlockData);
                routeSearchList.Add(fromBlock, topBlockData, Direction.Reverse(dir));
                if (topBlock.CompareTag("Goal"))
                {
                    VBirdHiyokoManager.Log($"ゴールが歩ける");
                    CanPlay = true;
                }
            }
            else
            {
                VBirdHiyokoManager.Log($"高くて歩けない");
                if (topBlock.TryGetComponent<BlockRouteData>(out BlockRouteData blockData))
                {
                    collideBlockList.Add(blockData);
                }
            }
        }

        Collider GetHighestCollider(int count)
        {
            // 一番上の高さを調べる
            float floorTop = results[0].bounds.max.y;
            int index = 0;
            for (int i = 1; i < count; i++)
            {
                if (floorTop < results[i].bounds.max.y)
                {
                    floorTop = results[i].bounds.max.y;
                    index = i;
                }
            }

            return results[index];
        }

        /// <summary>
        /// プレイヤーがすぐ隣にいる時の押せるかどうか判定。
        /// </summary>
        /// <returns>押せる状態なら true</returns>

        bool CanPushFromNext(BlockRouteData block)
        {
            // プレイヤーが隣にいないのでチェック不要
            if (!block.IsNextToThePlayer) return false;

            // プレイヤーが隣の時、前後の内積の絶対値が1なら前後にある
            var toPlayer = PiyoBehaviour.Instance.RigidbodyPosition - block.CenterTop;
            float dot = Vector3.Dot(toPlayer, Vector3.forward);

            // プレイヤーから見て前後にある時、前後のいずれかに押せれば押せる
            if ((Mathf.Abs(dot) > 0.9f)
                && (block.CanPush(Direction.Type.Forward)
                    || block.CanPush(Direction.Type.Back)))
            {
                return true;
            }

            // 左右のいずれかに押せれば押せる
            if (block.CanPush(Direction.Type.Left)
                || block.CanPush(Direction.Type.Right))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 左右方向に押せる可能性があるかを返す。
        /// </summary>
        /// <param name="block">確認対象のブロック</param>
        /// <returns>押せる可能性あり true</returns>
        bool CanXPush(BlockRouteData block)
        {
            return CanPush(block, Direction.Type.Left, Direction.Type.Right);
        }

        /// <summary>
        /// 前後方向に押せる可能性があるかを返す。
        /// </summary>
        /// <param name="block">確認対象のブロック</param>
        /// <returns>押せる可能性あり true</returns>
        bool CanZPush(BlockRouteData block)
        {
            return CanPush(block, Direction.Type.Back, Direction.Type.Forward);
        }

        /// <summary>
        /// 引数に従って、指定方向に押せる可能性がある時、trueを返す。
        /// </summary>
        /// <param name="block">確認対象のブロック</param>
        /// <param name="neg">マイナス方向</param>
        /// <param name="pos">プラス方向</param>
        /// <returns>押せる可能性がある時、true</returns>
        bool CanPush(BlockRouteData block, Direction.Type neg, Direction.Type pos)
        {
            // 前後両方押せればOK
            return block.CanPush(neg) && block.CanPush(pos);
        }

        /// <summary>
        /// ルートを全てクリアする。
        /// </summary>
        public void ClearRoute()
        {
            VBirdHiyokoManager.Log($"ClearRoute");
            RootBlock = null;
            BlockRouteData.ClearAll();
            IsSearched = false;
        }

        /// <summary>
        /// 前後のいずれかでも歩けるなら true
        /// </summary>
        /// <param name="block">確認対象のブロック</param>
        /// <returns>歩ける方向があれば true</returns>
        bool CanXWalk(BlockRouteData block)
        {
            return CanWalk(block.LinkedBlock[(int)Direction.Type.Left])
                || CanWalk(block.LinkedBlock[(int)Direction.Type.Right]);
        }

        /// <summary>
        /// 前後のいずれかでも歩けるなら true
        /// </summary>
        /// <param name="block">確認対象のブロック</param>
        /// <returns>歩ける方向があれば true</returns>
        bool CanZWalk(BlockRouteData block)
        {
            return CanWalk(block.LinkedBlock[(int)Direction.Type.Forward])
                || CanWalk(block.LinkedBlock[(int)Direction.Type.Back]);
        }

        /// <summary>
        /// 指定のブロックがあった歩行可能な時、true
        /// </summary>
        /// <param name="nextBlock">チェックしたいブロック</param>
        /// <returns>ブロックがあって歩行可能なら true</returns>
        bool CanWalk(BlockRouteData nextBlock)
        {
            return ((nextBlock != null) && nextBlock.CanWalk);
        }
    }
}