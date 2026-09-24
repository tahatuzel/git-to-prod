<script setup lang="ts">
import type { TodoFilter, TodoItem, TodoItemInput } from '~/types/todo'

const {
  items,
  loading,
  creating,
  pendingIds,
  errorMessage,
  refresh,
  create,
  update,
  remove
} = useTodoItems()

const filter = ref<TodoFilter>('all')
const creationResetKey = ref(0)
const openCount = computed(() => items.value.filter(item => !item.isCompleted).length)
const completedCount = computed(() => items.value.length - openCount.value)
const filteredItems = computed(() => {
  if (filter.value === 'open') return items.value.filter(item => !item.isCompleted)
  if (filter.value === 'completed') return items.value.filter(item => item.isCompleted)
  return items.value
})

async function addTodo(input: TodoItemInput) {
  if (await create(input)) creationResetKey.value += 1
}

function updateTodo(id: number, input: TodoItemInput) {
  return update(id, input)
}

function toggleTodo(item: TodoItem, isCompleted: boolean) {
  return update(item.id, {
    title: item.title,
    description: item.description,
    isCompleted
  })
}

onMounted(refresh)
</script>

<template>
  <main class="min-h-screen bg-muted/30">
    <UContainer class="max-w-3xl py-10 sm:py-16">
      <TodoHeader
        :open-count="openCount"
        :total-count="items.length"
      />

      <UAlert
        v-if="errorMessage"
        class="mb-6"
        color="error"
        variant="subtle"
        icon="i-lucide-circle-alert"
        title="İstek tamamlanamadı"
        :description="errorMessage"
      >
        <template #actions>
          <UButton
            label="Yeniden dene"
            color="error"
            variant="ghost"
            size="sm"
            @click="refresh"
          />
        </template>
      </UAlert>

      <UCard class="mb-8">
        <TodoCreateForm
          :loading="creating"
          :reset-key="creationResetKey"
          @create="addTodo"
        />
      </UCard>

      <section aria-labelledby="tasks-title">
        <div class="mb-4 flex flex-wrap items-center justify-between gap-3">
          <div>
            <h2
              id="tasks-title"
              class="text-lg font-semibold text-highlighted"
            >
              Görevlerin
            </h2>
            <p class="text-sm text-muted">
              {{ openCount }} açık · {{ completedCount }} tamamlandı
            </p>
          </div>

          <TodoFilter v-model="filter" />
        </div>

        <TodoList
          :items="filteredItems"
          :loading="loading"
          :pending-ids="pendingIds"
          @toggle="toggleTodo"
          @update="updateTodo"
          @delete="remove"
        />
      </section>
    </UContainer>
  </main>
</template>
