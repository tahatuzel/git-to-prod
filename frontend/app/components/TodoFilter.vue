<script setup lang="ts">
import type { TodoFilter } from '~/types/todo'

defineProps<{
  modelValue: TodoFilter
}>()

const emit = defineEmits<{
  'update:modelValue': [filter: TodoFilter]
}>()

const filters: { label: string, value: TodoFilter }[] = [
  { label: 'Tümü', value: 'all' },
  { label: 'Açık', value: 'open' },
  { label: 'Tamamlanan', value: 'completed' }
]
</script>

<template>
  <div
    class="flex gap-1 rounded-lg bg-elevated p-1"
    role="group"
    aria-label="Görev filtresi"
  >
    <UButton
      v-for="item in filters"
      :key="item.value"
      size="sm"
      :label="item.label"
      :color="modelValue === item.value ? 'primary' : 'neutral'"
      :variant="modelValue === item.value ? 'subtle' : 'ghost'"
      @click="emit('update:modelValue', item.value)"
    />
  </div>
</template>
