import { test, expect, Page } from '@playwright/test';

const DUMMY_PROFILE = {
  name: 'Doe',
  firstname: 'John',
  gender: 'Male',
  country: 'Belgium',
};

async function waitForBlazor(page: Page) {
  // Wait for Blazor interactive mode to fully connect
  await page.waitForLoadState('networkidle');
  await page.waitForSelector('.profile-container', { timeout: 10_000 });
}

test.describe('Profile page', () => {
  test('shows profile data or adds a dummy profile and verifies persistence', async ({ page }) => {
    // ── 1. Navigate to profile ──────────────────────────────────────────
    await page.goto('/profile');
    await waitForBlazor(page);

    const isEmpty = await page.locator('.profile-empty').isVisible();

    if (isEmpty) {
      // ── 2a. No profile yet — fill in dummy data ──────────────────────
      console.log('No profile found — creating dummy profile...');

      await page.getByRole('button', { name: 'Edit' }).click();
      await expect(page.locator('.profile-edit')).toBeVisible();

      await page.fill('#name', DUMMY_PROFILE.name);
      await page.fill('#firstname', DUMMY_PROFILE.firstname);
      await page.selectOption('#gender', DUMMY_PROFILE.gender);
      await page.selectOption('#country', DUMMY_PROFILE.country);

      await page.getByRole('button', { name: 'Save' }).click();

      // ── 3. Success message appears and edit form is gone ─────────────
      await expect(page.locator('.message.success')).toBeVisible({ timeout: 5_000 });
      await expect(page.locator('.profile-edit')).not.toBeVisible();
    } else {
      // ── 2b. Profile already exists — just log the current values ──────
      console.log('Profile already present — skipping creation.');
    }

    // ── 4. Verify view mode shows the expected data ───────────────────
    const profileView = page.locator('.profile-view');
    await expect(profileView).toBeVisible();

    await expect(profileView.locator('.profile-field').nth(0).locator('.value'))
      .toHaveText(DUMMY_PROFILE.name);
    await expect(profileView.locator('.profile-field').nth(1).locator('.value'))
      .toHaveText(DUMMY_PROFILE.firstname);
    await expect(profileView.locator('.profile-field').nth(2).locator('.value'))
      .toHaveText(DUMMY_PROFILE.gender);
    await expect(profileView.locator('.profile-field').nth(3).locator('.value'))
      .toHaveText(DUMMY_PROFILE.country);

    // ── 5. Navigate away and back — verify persistence ────────────────
    await page.goto('/');
    await page.goto('/profile');
    await waitForBlazor(page);

    await expect(page.locator('.profile-view')).toBeVisible();
    await expect(page.locator('.profile-empty')).not.toBeVisible();

    await expect(page.locator('.profile-field').nth(0).locator('.value'))
      .toHaveText(DUMMY_PROFILE.name);
    await expect(page.locator('.profile-field').nth(1).locator('.value'))
      .toHaveText(DUMMY_PROFILE.firstname);
  });
});
