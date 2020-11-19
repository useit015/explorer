using DCL.Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityListAdapter : MonoBehaviour
{
    public Color entitySelectedColor, entityUnselectedColor;
    public Color iconsSelectedColor, iconsUnselectedColor;
    public TextMeshProUGUI nameTxt;
    public Image selectedImg, lockImg, showImg;
    public System.Action<BuilderInWorldEntityListController.EntityAction, DCLBuilderInWorldEntity, EntityListAdapter> OnActionInvoked;
    DCLBuilderInWorldEntity currentEntity;


    private void OnDestroy()
    {
        if (currentEntity != null)
        {
            currentEntity.onStatusUpdate -= SetInfo;
            currentEntity.OnDelete -= DeleteAdapter;
        }
    }

    public void SetContent(DCLBuilderInWorldEntity decentrelandEntity)
    {
        if(currentEntity != null)
        {
            currentEntity.onStatusUpdate -= SetInfo;
            currentEntity.OnDelete -= DeleteAdapter;
        }
        currentEntity = decentrelandEntity;
        currentEntity.onStatusUpdate += SetInfo;
        currentEntity.OnDelete += DeleteAdapter;

        SetInfo(decentrelandEntity);
    }

    public void SelectOrDeselect()
    {
        OnActionInvoked?.Invoke(BuilderInWorldEntityListController.EntityAction.SELECT,currentEntity, this);
    }

    public void ShowOrHide()
    {
         OnActionInvoked?.Invoke(BuilderInWorldEntityListController.EntityAction.SHOW, currentEntity, this);
    }

    public void LockOrUnlock()
    {
        OnActionInvoked?.Invoke(BuilderInWorldEntityListController.EntityAction.LOCK, currentEntity, this);
    }

    public void DeleteEntity()
    {
        OnActionInvoked?.Invoke(BuilderInWorldEntityListController.EntityAction.DELETE, currentEntity, this);
    }

    void SetInfo(DCLBuilderInWorldEntity entityToEdit)
    {
        if (this != null)
        {
            nameTxt.text = entityToEdit.rootEntity.entityId;
            if (entityToEdit.IsVisible)
                showImg.color = iconsSelectedColor;
            else
                showImg.color = iconsUnselectedColor;

            if (entityToEdit.IsLocked)
                lockImg.color = iconsSelectedColor;
            else
                lockImg.color = iconsUnselectedColor;


            if (entityToEdit.IsSelected)
                selectedImg.color = entitySelectedColor;
            else
                selectedImg.color = entityUnselectedColor;
        }
    }

    void DeleteAdapter(DCLBuilderInWorldEntity entityToEdit)
    {
        if (this != null)
            if (entityToEdit.entityUniqueId == currentEntity.entityUniqueId)
                Destroy(gameObject);
    }
}
