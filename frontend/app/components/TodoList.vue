<script setup lang="ts">
import type { TodoItem, TodoItemInput } from '~/types/todo'

defineProps<{
  items: TodoItem[]
  loading: boolean
  pendingIds: number[]
}>()

const emit = defineEmits<{
  toggle: [item: TodoItem, isCompleted: boolean]
  update: [id: number, input: TodoItemInput]
  delete: [id: number]
}>()
</script>

<template>
  <div
    v-if="loading"
    class="space-y-3"
    aria-label="Görevler yükleniyor"
  >
    <USkeleton
      v-for="row in 3"
      :key="row"
      class="h-20 w-full rounded-xl"
    />
  </div>

  <UCard
    v-else-if="items.length === 0"
    class="text-center"
  >
    <UIcon
      name="i-lucide-clipboard-check"
      class="mx-auto mb-3 size-8 text-primary"
    />
    <p class="font-medium text-highlighted">
      Burada şimdilik bir şey yok
    </p>
    <p class="mt-1 text-sm text-muted">
      Yeni bir görev ekleyerek başlayabilirsin.
    </p>
  </UCard>

  <ul
    v-else
    class="space-y-3"
  >
    <li
      v-for="item in items"
      :key="item.id"
    >
      <TodoItem
        :item="item"
        :loading="pendingIds.includes(item.id)"
        @toggle="isCompleted => emit('toggle', item, isCompleted)"
        @update="input => emit('update', item.id, input)"
        @delete="emit('delete', item.id)"
      />
    </li>
  </ul>
</template>
