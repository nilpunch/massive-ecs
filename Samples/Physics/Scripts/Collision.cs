namespace Massive.Samples.Physics
{
	public static class Collision
	{
		// private static void SolveContacts(in Particle a, in Particle b)
		// {
		// 	constexpr float response_coef = 1.0f;
		// 	constexpr float eps           = 0.0001f;
		// 	PhysicObject& obj_1 = objects.data[atom_1_idx];
		// 	PhysicObject& obj_2 = objects.data[atom_2_idx];
		// 	const Vec2 o2_o1  = obj_1.position - obj_2.position;
		// 	const float dist2 = o2_o1.x * o2_o1.x + o2_o1.y * o2_o1.y;
		// 	if (dist2 < 1.0f && dist2 > eps) {
		// 		const float dist          = sqrt(dist2);
		// 		// Radius are all equal to 1.0f
		// 		const float delta  = response_coef * 0.5f * (1.0f - dist);
		// 		const Vec2 col_vec = (o2_o1 / dist) * delta;
		// 		obj_1.position += col_vec;
		// 		obj_2.position -= col_vec;
		// 	}
		// }
	}
}