import { loadTestParcel } from '../testHelpers'
import { future } from 'fp-future'
import { expect } from 'chai'
import { ScriptingHostEvents } from 'decentraland-rpc/lib/host'
import { sleep } from 'atomicHelpers/sleep'

loadTestParcel('Unmount parcelScenes due limits in gltf', -1, 36, function(root, parcelScene, worker) {
  const didTriggerLimits = future()
  const disableFuture = future()

  it('waits for the system to load', async () => {
    const ret = await parcelScene
    const system = await (await worker).system

    expect(system.isEnabled).to.eq(true, 'system should be enabled')

    system.on(ScriptingHostEvents.systemDidUnmount, () => {
      disableFuture.resolve(ScriptingHostEvents.systemDidUnmount)
    })

    ret.context.on('limitsExceeded', evt => {
      didTriggerLimits.resolve(evt)
    })

    ret.checkLimits({
      limit: ret.context.metricsLimits,
      given: ret.context.metrics
    })
  })

  it('should trigger exceeded limits', async function() {
    this.timeout(10000)

    await didTriggerLimits

    // let's give some time to babylon so it can remove the things from the scene
    await sleep(3000)
  })

  it('should unmount the system', async () => {
    await disableFuture
  })
})
