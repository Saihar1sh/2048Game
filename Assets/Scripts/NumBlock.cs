using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ElRaccoone.Tweens;


public class NumBlock : MonoBehaviour
{
    public bool needDestroy = false;

    [SerializeField] TextMeshPro valueTxt;

    int value = 0;

    Block from, target;
    
    private bool combine = false;

    //properties
    public int Value 
    {   get { return value; } 
        set 
        {
            this.value = value;
            valueTxt.text = value.ToString(); 
        } 
    }
    public void Init(int numValue)
    {
        Value = numValue;
        PlayScaleAnimation(()=> { });
    }
    /// <summary>
    /// increases local scale by 25% and then sets back to original scale in 0.3 secs.
    /// </summary>
    private void PlayScaleAnimation(System.Action onCompleteAction)
    {
        var tween = gameObject.TweenLocalScale(new Vector3(1.25f, 1.25f, 1.25f), 0.15f).SetPingPong().SetEase(ElRaccoone.Tweens.Core.EaseType.Linear).SetOnComplete(onCompleteAction);
    }

    public void MoveToBlock(Block from, Block to)
    {
        combine = false;
        this.from = from;
        this.target = to;
    }

    public void CombineToBlock(Block from, Block to)
    {
        combine = true;
        this.from = from;
        this.target = to;
    }
    public void OnEndMove()
    {
        if (target != null)
        {
            target.OccupyThisBlock(this);
            if (combine)
            {
                NumBlock numBlock = target.MergedBlockValueChange();
                if (numBlock)
                    numBlock.PlayScaleAnimation(() => {
                        this.needDestroy = true;
                        this.target = null;
                        this.from = null;
                    });
                this.gameObject.SetActive(false);
            }
            else
            {
                this.from = null;
                this.target = null;
            }
        }
    }

    public void StartMoveAnimation()
    {
        if (target != null)
        {
            this.name = target.index.ToString();
            var tween = this.transform.TweenLocalPosition(target.transform.position, 0.1f);
            tween.SetOnComplete(()=> OnEndMove());
        }

    }

}
