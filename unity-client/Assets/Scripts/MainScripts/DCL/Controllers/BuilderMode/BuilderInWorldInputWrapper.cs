using Builder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuilderInWorldInputWrapper : MonoBehaviour
{
    public LayerMask layerToStopClick;
    public float msClickThreshold = 200;
    public float movementClickThreshold = 50;

    public event Action<int, Vector3> OnMouseClick;
    public event Action<int, Vector3> OnMouseDown;
    public event Action<int, Vector3> OnMouseUp;

    public event Action<float> OnMouseWheel;

    public event OnMouseDragDelegate OnMouseDrag;
    public event OnMouseDragDelegateRaw OnMouseDragRaw;


    public delegate void OnMouseDragDelegate(int buttonId, Vector3 position, float axisX, float axisY);
    public delegate void OnMouseDragDelegateRaw(int buttonId, Vector3 position, float axisX, float axisY);


    private float lastTimeMouseDown = 0;
    private Vector3 lastMousePosition;
    private bool canInputBeMade = true;
    private bool currentClickIsOnUi = false;

    private void Awake()
    {
        DCLBuilderInput.OnMouseDrag += MouseDrag;
        DCLBuilderInput.OnMouseRawDrag += MouseRawDrag;
        DCLBuilderInput.OnMouseWheel += MouseWheel;
        DCLBuilderInput.OnMouseDown += MouseDown;
        DCLBuilderInput.OnMouseUp += MouseUp;
    }

    public void StopInput()
    {
        canInputBeMade = false;
    }

    public void ResumeInput()
    {
        canInputBeMade = true;
    }

    private void MouseUp(int buttonId, Vector3 mousePosition)
    {
        currentClickIsOnUi = false;

        if (!canInputBeMade)
            return;

        if (!BuilderInWorldUtils.IsPointerOverUIElement() && !BuilderInWorldUtils.IsPointerOverMaskElement(layerToStopClick))
        {
            OnMouseUp?.Invoke(buttonId, mousePosition);
            if (Vector3.Distance(mousePosition, lastMousePosition) >= movementClickThreshold)
                return;
            if (Time.unscaledTime >= lastTimeMouseDown + msClickThreshold / 1000)
                return;
            OnMouseClick?.Invoke(buttonId, mousePosition);
        }
    }

    private void MouseDown(int buttonId, Vector3 mousePosition)
    {
        lastTimeMouseDown = Time.unscaledTime;
        lastMousePosition = mousePosition;
        currentClickIsOnUi = BuilderInWorldUtils.IsPointerOverUIElement();

        if (!canInputBeMade)
            return;
        if (!currentClickIsOnUi && !BuilderInWorldUtils.IsPointerOverMaskElement(layerToStopClick))
            OnMouseDown?.Invoke(buttonId, mousePosition);
    }

    private void MouseWheel(float axisValue)
    {
        if (!canInputBeMade)
            return;
        if (!BuilderInWorldUtils.IsPointerOverUIElement())
            OnMouseWheel?.Invoke(axisValue);
    }

    private void MouseDrag(int buttonId, Vector3 mousePosition, float axisX, float axisY)
    {
        if (!CanDrag())
            return;

        OnMouseDrag?.Invoke(buttonId, mousePosition, axisX, axisY);
    }

    private void MouseRawDrag(int buttonId, Vector3 mousePosition, float axisX, float axisY)
    {
        if (!CanDrag())
            return;

        OnMouseDragRaw?.Invoke(buttonId, mousePosition, axisX, axisY);
    }

    private bool CanDrag()
    {
        if (!canInputBeMade ||
            currentClickIsOnUi ||
            BuilderInWorldUtils.IsPointerOverUIElement())
            return false;

        return true;
    }
}
