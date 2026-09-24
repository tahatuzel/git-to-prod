import assert from 'node:assert/strict'
import { spawn } from 'node:child_process'
import { createServer } from 'node:http'
import { once } from 'node:events'
import { resolve } from 'node:path'
import { setTimeout } from 'node:timers/promises'
import test from 'node:test'

async function listen(server) {
  server.listen(0, '127.0.0.1')
  await once(server, 'listening')
  return server.address().port
}

test('production Nuxt server proxies todo HTTP requests and responses', async (t) => {
  const backend = createServer(async (request, response) => {
    const chunks = []
    for await (const chunk of request) chunks.push(chunk)

    const result = {
      method: request.method,
      url: request.url,
      body: Buffer.concat(chunks).toString()
    }
    response.writeHead(request.url === '/api/todo-items/404' ? 422 : 200, {
      'content-type': 'application/json'
    })
    response.end(JSON.stringify(result))
  })
  const backendPort = await listen(backend)
  t.after(() => new Promise((resolve, reject) => {
    backend.close(error => error ? reject(error) : resolve())
  }))

  const portReservation = createServer()
  const frontendPort = await listen(portReservation)
  await new Promise(resolve => portReservation.close(resolve))

  const frontend = spawn(process.execPath, [resolve('.output/server/index.mjs')], {
    env: {
      ...process.env,
      HOST: '127.0.0.1',
      PORT: String(frontendPort),
      NUXT_API_BASE_URL: `http://127.0.0.1:${backendPort}`
    },
    stdio: ['ignore', 'pipe', 'pipe']
  })
  let output = ''
  for (const stream of [frontend.stdout, frontend.stderr]) {
    stream.on('data', (chunk) => {
      output += chunk.toString()
    })
  }
  t.after(async () => {
    if (frontend.exitCode === null && frontend.signalCode === null) {
      const exited = once(frontend, 'exit')
      frontend.kill()
      await exited
    }
  })

  const baseUrl = `http://127.0.0.1:${frontendPort}`
  let ready = false
  for (let attempt = 0; attempt < 100; attempt++) {
    if (frontend.exitCode !== null) break
    try {
      const response = await fetch(baseUrl)
      if (response.ok) {
        ready = true
        break
      }
    } catch {
      await setTimeout(100)
    }
  }
  assert.ok(ready, `Nuxt server did not start:\n${output}`)

  const cases = [
    { path: '/api/todo-items?filter=open', method: 'GET', status: 200 },
    { path: '/api/todo-items', method: 'POST', body: { title: 'Test' }, status: 200 },
    { path: '/api/todo-items/7', method: 'PUT', body: { title: 'Updated' }, status: 200 },
    { path: '/api/todo-items/7', method: 'DELETE', status: 200 },
    { path: '/api/todo-items/404', method: 'GET', status: 422 }
  ]

  for (const { path, method, body, status } of cases) {
    const response = await fetch(`${baseUrl}${path}`, {
      method,
      headers: body ? { 'content-type': 'application/json' } : undefined,
      body: body ? JSON.stringify(body) : undefined
    })
    assert.equal(response.status, status)
    assert.deepEqual(await response.json(), {
      method,
      url: path,
      body: body ? JSON.stringify(body) : ''
    })
  }
})
