import { getApiErrorMessage, todoItemsApi } from '~/services/todo-items'
import type { TodoItem, TodoItemInput } from '~/types/todo'

export function useTodoItems() {
  const items = ref<TodoItem[]>([])
  const loading = ref(true)
  const creating = ref(false)
  const pendingIds = ref<number[]>([])
  const errorMessage = ref('')

  async function refresh() {
    loading.value = true
    errorMessage.value = ''

    try {
      items.value = await todoItemsApi.getAll()
    } catch (error) {
      errorMessage.value = getApiErrorMessage(error)
    } finally {
      loading.value = false
    }
  }

  async function create(input: TodoItemInput) {
    creating.value = true
    errorMessage.value = ''

    try {
      const item = await todoItemsApi.create(input)
      items.value = [item, ...items.value]
      return true
    } catch (error) {
      errorMessage.value = getApiErrorMessage(error)
      return false
    } finally {
      creating.value = false
    }
  }

  async function update(id: number, input: TodoItemInput) {
    if (pendingIds.value.includes(id)) return

    pendingIds.value = [...pendingIds.value, id]
    errorMessage.value = ''

    try {
      const item = await todoItemsApi.update(id, input)
      items.value = items.value.map(current => current.id === id ? item : current)
    } catch (error) {
      errorMessage.value = getApiErrorMessage(error)
    } finally {
      pendingIds.value = pendingIds.value.filter(currentId => currentId !== id)
    }
  }

  async function remove(id: number) {
    if (pendingIds.value.includes(id)) return

    pendingIds.value = [...pendingIds.value, id]
    errorMessage.value = ''

    try {
      await todoItemsApi.delete(id)
      items.value = items.value.filter(item => item.id !== id)
    } catch (error) {
      errorMessage.value = getApiErrorMessage(error)
    } finally {
      pendingIds.value = pendingIds.value.filter(currentId => currentId !== id)
    }
  }

  return {
    items,
    loading,
    creating,
    pendingIds,
    errorMessage,
    refresh,
    create,
    update,
    remove
  }
}
