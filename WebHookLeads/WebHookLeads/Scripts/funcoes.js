// Método que verifica se o objeto é diferente de NULL, undefined e vazio ''.
// vc pode passar tanto uma variável javascript ou um objeto jquery q o método se vira e acha o valor. (Não precisa passar obj.val() )
function hasValue(obj) {
	if (obj instanceof jQuery) {
		obj = obj.val();
	}

	return	obj != null &&
			obj != undefined &&
			obj != ''
}