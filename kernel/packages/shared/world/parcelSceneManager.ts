import { initParcelSceneWorker } from 'decentraland-loader/lifecycle/manager'
import { ScriptingTransport } from 'decentraland-rpc/lib/common/json-rpc/types'
import { sceneLifeCycleObservable } from '../../decentraland-loader/lifecycle/controllers/scene'
import { queueTrackingEvent } from '../analytics'
import { globalSignalSceneFail, globalSignalSceneLoad, globalSignalSceneStart } from '../loading/actions'
import { clearForegroundTimeout, setForegroundTimeout } from '../timers/index'
import { EnvironmentData, ILand, InstancedSpawnPoint, LoadableParcelScene } from '../types'
import { ParcelSceneAPI } from './ParcelSceneAPI'
import { parcelObservable, teleportObservable } from './positionThings'
import { SceneWorker } from './SceneWorker'
import { SceneSystemWorker } from './SceneSystemWorker'
import { renderStateObservable } from './worldState'
import { ILandToLoadableParcelScene } from 'shared/selectors'

export type EnableParcelSceneLoadingOptions = {
  parcelSceneClass: { new (x: EnvironmentData<LoadableParcelScene>): ParcelSceneAPI }
  preloadScene: (parcelToLoad: ILand) => Promise<any>
  onPositionSettled?: (spawnPoint: InstancedSpawnPoint) => void
  onLoadParcelScenes?(x: ILand[]): void
  onUnloadParcelScenes?(x: ILand[]): void
  onPositionUnsettled?(): void
}

export const loadedSceneWorkers = new Map<string, SceneWorker>()
declare var window: any
window['sceneWorkers'] = loadedSceneWorkers

/**
 * Retrieve the Scene based on it's ID, usually RootCID
 */
export function getSceneWorkerBySceneID(sceneId: string) {
  return loadedSceneWorkers.get(sceneId)
}

/**
 * Returns the id of the scene, usually the RootCID
 */
export function getParcelSceneID(parcelScene: ParcelSceneAPI) {
  return parcelScene.data.sceneId
}

/** Stops non-persistent scenes (i.e UI scene) */
export function stopParcelSceneWorker(worker: SceneWorker) {
  if (worker && !worker.isPersistent()) {
    forceStopParcelSceneWorker(worker)
  }
}

export function forceStopParcelSceneWorker(worker: SceneWorker) {
  const sceneId = worker.getSceneId()
  worker.dispose()
  loadedSceneWorkers.delete(sceneId)
}

export function loadParcelScene(
  parcelScene: ParcelSceneAPI,
  transport?: ScriptingTransport,
  persistent: boolean = false
) {
  const sceneId = getParcelSceneID(parcelScene)

  let parcelSceneWorker = loadedSceneWorkers.get(sceneId)

  if (!parcelSceneWorker) {
    parcelSceneWorker = new SceneSystemWorker(parcelScene, transport, persistent)

    setNewParcelScene(sceneId, parcelSceneWorker)
  }

  return parcelSceneWorker
}

export function setNewParcelScene(sceneId: string, worker: SceneWorker) {
  loadedSceneWorkers.set(sceneId, worker)
}

export async function enableParcelSceneLoading(options: EnableParcelSceneLoadingOptions) {
  const ret = await initParcelSceneWorker()

  ret.on('Scene.shouldPrefetch', async (opts: { sceneId: string }) => {
    const parcelSceneToLoad = await ret.getParcelData(opts.sceneId)

    // start and await prefetch
    await options.preloadScene(parcelSceneToLoad)

    // continue with the loading
    ret.notify('Scene.prefetchDone', opts)
  })

  ret.on('Scene.shouldStart', async (opts: { sceneId: string }) => {
    const sceneId = opts.sceneId
    const parcelSceneToStart = await ret.getParcelData(sceneId)
    globalSignalSceneLoad(sceneId)
    // create the worker if don't exist
    if (!getSceneWorkerBySceneID(sceneId)) {
      const parcelScene = new options.parcelSceneClass(ILandToLoadableParcelScene(parcelSceneToStart))
      parcelScene.data.useFPSThrottling = true
      loadParcelScene(parcelScene)
    }

    let timer: any

    const observer = sceneLifeCycleObservable.add((sceneStatus) => {
      if (sceneStatus.sceneId === sceneId) {
        sceneLifeCycleObservable.remove(observer)
        clearForegroundTimeout(timer)
        ret.notify('Scene.status', sceneStatus)
        globalSignalSceneStart(sceneId)
      }
    })

    // tell the engine to load the parcel scene
    if (options.onLoadParcelScenes) {
      options.onLoadParcelScenes([await ret.getParcelData(sceneId)])
    }

    timer = setForegroundTimeout(() => {
      const worker = getSceneWorkerBySceneID(sceneId)
      if (worker && !worker.hasSceneStarted()) {
        sceneLifeCycleObservable.remove(observer)
        globalSignalSceneFail(sceneId)
        ret.notify('Scene.status', { sceneId, status: 'failed' })
      }
    }, 90000)
  })

  ret.on('Scene.shouldUnload', async (opts: { sceneId: string }) => {
    const worker = loadedSceneWorkers.get(opts.sceneId)
    if (!worker) {
      return
    }
    stopParcelSceneWorker(worker)
    if (options.onUnloadParcelScenes) {
      const globalStore = (global as any)['globalStore']
      globalStore.dispatch({ type: 'Unloaded scene', sceneId: opts.sceneId })
      options.onUnloadParcelScenes([await ret.getParcelData(opts.sceneId)])
    }
  })

  ret.on('Position.settled', async (opts: { spawnPoint: InstancedSpawnPoint }) => {
    if (options.onPositionSettled) {
      options.onPositionSettled(opts.spawnPoint)
    }
  })

  ret.on('Position.unsettled', () => {
    if (options.onPositionUnsettled) {
      options.onPositionUnsettled()
    }
    renderStateObservable.notifyObservers(false)
  })

  ret.on('Event.track', (event: { name: string; data: any }) => {
    queueTrackingEvent(event.name, event.data)
  })

  teleportObservable.add((position: { x: number; y: number }) => {
    ret.notify('User.setPosition', { position, teleported: true })
  })

  parcelObservable.add((obj) => {
    // immediate reposition should only be broadcasted to others, otherwise our scene reloads
    if (obj.immediate) return

    ret.notify('User.setPosition', { position: obj.newParcel, teleported: false })
  })
}
