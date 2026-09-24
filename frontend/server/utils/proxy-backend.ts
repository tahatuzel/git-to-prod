export function proxyBackendRequest(event: Parameters<typeof proxyRequest>[0]) {
  const { apiBaseUrl } = useRuntimeConfig()
  const requestUrl = getRequestURL(event)
  const target = new URL(`${requestUrl.pathname}${requestUrl.search}`, apiBaseUrl)

  return proxyRequest(event, target.toString())
}
