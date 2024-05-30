var app = new Vue({
    el:"#app",
    data:{
        clientInfo: {},
        error: null
    },
    methods:{
        getData: function(){
            axios.get("/api/clients/3")
            .then(function (response) {
                //get client ifo
                app.clientInfo = response.data;
            })
            .catch(function (error) {
                // handle error
                app.error = error;
            })
        },
        formatDate: function (date) {
            console.log(date)
            return new Date(date).toLocaleDateString('en-gb');
        }
    },
    mounted: function(){
        this.getData();
    }
})