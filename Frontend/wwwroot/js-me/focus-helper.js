

if(window.visualViewport){
    window.visualViewport.addEventListener("resize", () => {
        const wrapper = document.querySelector(".form-wrapper");
        if(!wrapper) return;
        if(window.innerHeight > window.visualViewport.height){
            wrapper.classList.add("keyboard-open");
        }else{
            wrapper.classList.remove("keyboard-open");
        }
    })
}