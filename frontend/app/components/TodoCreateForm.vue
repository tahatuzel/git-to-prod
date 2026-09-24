<script setup lang="ts">
import type { TodoItemInput } from '~/types/todo'

const props = defineProps<{
  loading: boolean
  resetKey: number
}>()

const emit = defineEmits<{
  create: [input: TodoItemInput]
}>()

const title = ref('')
const description = ref('')

watch(() => props.resetKey, () => {
  title.value = ''
  description.value = ''
})

function submit() {
  const cleanTitle = title.value.trim()
  if (!cleanTitle) return

  emit('create', {
    title: cleanTitle,
    description: description.value.trim(),
    isCompleted: false
  })
}
</script>

<template>
  <form
    class="space-y-4"
    @submit.prevent="submit"
  >
    <div class="flex flex-col gap-3 sm:flex-row">
      <UInput
        v-model="title"
        class="flex-1"
        placeholder="Yeni bir görev ekle"
        aria-label="Görev başlığı"
        maxlength="200"
        required
        autofocus
      />
      <UButton
        type="submit"
        label="Görev ekle"
        icon="i-lucide-plus"
        :loading="loading"
        :disabled="!title.trim()"
      />
    </div>

    <UTextarea
      v-model="description"
      class="w-full"
      placeholder="Açıklama ekle (isteğe bağlı)"
      aria-label="Görev açıklaması"
      :rows="2"
      autoresize
    />
  </form>
</template>
