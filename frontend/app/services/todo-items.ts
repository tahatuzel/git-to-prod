import type { ApiEnvelope, TodoItem, TodoItemInput } from '~/types/todo'

interface RequestOptions {
  method: 'POST' | 'PUT' | 'DELETE'
  body?: TodoItemInput
}

async function request<T>(url: string, options?: RequestOptions): Promise<T> {
  const envelope = await $fetch<ApiEnvelope<T>>(url, options)

  if (!envelope.result.isSuccess || envelope.response === null) {
    const message = envelope.errorResponse?.errors.map(error => error.message).join(' ')
    throw new Error(message || 'İstek tamamlanamadı.')
  }

  return envelope.response
}

export const todoItemsApi = {
  async getAll(): Promise<TodoItem[]> {
    const response = await request<{ items: TodoItem[] }>('/api/todo-items')
    return response.items
  },

  create(input: TodoItemInput): Promise<TodoItem> {
    return request<TodoItem>('/api/todo-items', {
      method: 'POST',
      body: input
    })
  },

  update(id: number, input: TodoItemInput): Promise<TodoItem> {
    return request<TodoItem>(`/api/todo-items/${id}`, {
      method: 'PUT',
      body: input
    })
  },

  delete(id: number): Promise<{ id: number }> {
    return request<{ id: number }>(`/api/todo-items/${id}`, {
      method: 'DELETE'
    })
  }
}

export function getApiErrorMessage(error: unknown): string {
  if (typeof error === 'object' && error !== null && 'data' in error) {
    const payload = (error as { data?: ApiEnvelope<unknown> }).data
    const messages = payload?.errorResponse?.errors.map(apiError => apiError.message)

    if (messages?.length) {
      return messages.join(' ')
    }
  }

  return error instanceof Error ? error.message : 'Beklenmeyen bir hata oluştu.'
}
