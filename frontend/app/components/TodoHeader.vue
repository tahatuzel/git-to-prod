<script setup lang="ts">
defineProps<{
  openCount: number
  totalCount: number
}>()
</script>

<template>
  <header class="mb-8 flex flex-wrap items-end justify-between gap-5">
    <div>
      <div class="mb-3 flex items-center gap-2 text-primary">
        <UIcon
          name="i-lucide-check-check"
          class="size-5"
        />
        <span class="text-xs font-semibold uppercase tracking-[0.18em]">Küçük adımlar</span>
      </div>
      <h1 class="text-3xl font-bold tracking-tight text-highlighted sm:text-4xl">
        Yapılacaklar
      </h1>
      <p class="mt-2 text-muted">
        Aklındakileri sıraya koy, gerisini tek tek hallet.
      </p>
    </div>

    <UBadge
      color="primary"
      variant="subtle"
      size="lg"
    >
      {{ openCount }} / {{ totalCount }} açık
    </UBadge>
  </header>
</template>
