<script setup lang="ts">
import type { TodoItem as TodoItemModel, TodoItemInput } from '~/types/todo'

const props = defineProps<{
  item: TodoItemModel
  loading: boolean
}>()

const emit = defineEmits<{
  toggle: [isCompleted: boolean]
  update: [input: TodoItemInput]
  delete: []
}>()

const editing = ref(false)
const title = ref(props.item.title)
const description = ref(props.item.description)

watch(() => props.item, (item) => {
  title.value = item.title
  description.value = item.description
  editing.value = false
})

function save() {
  const cleanTitle = title.value.trim()
  if (!cleanTitle) return

  emit('update', {
    title: cleanTitle,
    description: description.value.trim(),
    isCompleted: props.item.isCompleted
  })
}

function cancel() {
  title.value = props.item.title
  description.value = props.item.description
  editing.value = false
}

function updateCompletion(value: boolean | 'indeterminate') {
  if (typeof value === 'boolean') emit('toggle', value)
}
</script>

<template>
  <UCard :class="item.isCompleted && !editing ? 'opacity-70' : undefined">
    <form
      v-if="editing"
      class="space-y-3"
      @submit.prevent="save"
    >
      <UInput
        v-model="title"
        aria-label="Görev başlığı"
        maxlength="200"
        required
        :disabled="loading"
      />
      <UTextarea
        v-model="description"
        class="w-full"
        aria-label="Görev açıklaması"
        :rows="2"
        autoresize
        :disabled="loading"
      />
      <div class="flex justify-end gap-2">
        <UButton
          label="Vazgeç"
          color="neutral"
          variant="ghost"
          :disabled="loading"
          @click="cancel"
        />
        <UButton
          type="submit"
          label="Kaydet"
          icon="i-lucide-check"
          :loading="loading"
          :disabled="!title.trim()"
        />
      </div>
    </form>

    <div
      v-else
      class="flex items-start gap-3"
    >
      <UCheckbox
        class="mt-0.5"
        :model-value="item.isCompleted"
        :aria-label="item.isCompleted ? 'Görevi açık duruma getir' : 'Görevi tamamla'"
        :disabled="loading"
        @update:model-value="updateCompletion"
      />

      <div class="min-w-0 flex-1">
        <p
          class="break-words font-medium text-highlighted"
          :class="item.isCompleted ? 'line-through text-muted' : undefined"
        >
          {{ item.title }}
        </p>
        <p
          v-if="item.description"
          class="mt-1 whitespace-pre-wrap break-words text-sm text-muted"
        >
          {{ item.description }}
        </p>
      </div>

      <div class="flex shrink-0 items-center gap-1">
        <UButton
          icon="i-lucide-pencil"
          color="neutral"
          variant="ghost"
          size="sm"
          :disabled="loading"
          :aria-label="`${item.title} görevini düzenle`"
          @click="editing = true"
        />
        <UButton
          icon="i-lucide-trash-2"
          color="error"
          variant="ghost"
          size="sm"
          :loading="loading"
          :aria-label="`${item.title} görevini sil`"
          @click="emit('delete')"
        />
      </div>
    </div>
  </UCard>
</template>
